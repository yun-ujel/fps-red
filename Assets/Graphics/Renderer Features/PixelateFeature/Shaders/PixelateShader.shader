Shader "Screen/Pixelate"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            SamplerState sampler_point_clamp;
            SamplerState sampler_bilinear_clamp;

            float2 _BlockCount;
            float2 _BlockSize;
            float2 _HalfBlockSize;

            half4 PointPixelate(Texture2D tex, float2 UV)
            {
                float2 blockPos = floor(UV * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                return tex.Sample(sampler_point_clamp, blockCenter);
            }

            float _UpscaleRatio;

            float4 FilteredPixelate(Texture2D tex, float2 UV)
            {
                float2 blockPos = UV * _BlockCount;

                float2 blockOffset = clamp(frac(blockPos) * _UpscaleRatio, 0, 0.5) - clamp((1 - frac(blockPos)) * _UpscaleRatio, 0, 0.5);

                float2 sampleUV = (floor(blockPos) + 0.5 + blockOffset) * _BlockSize;

                return tex.Sample(sampler_bilinear_clamp, sampleUV);
            }

            float _DepthThreshold;
            float _NormalsThreshold;

            float3 _NormalEdgeBias;

            float _DepthEdgeStrength;
            float _NormalEdgeStrength;

            TEXTURE2D(_CameraOpaqueTexture);
            TEXTURE2D(_CameraDepthTexture);
            TEXTURE2D(_CameraNormalsTexture);

            float3 GetNormals(float2 UV)
            {
                return PointPixelate(_CameraNormalsTexture, UV);
            }

            float GetDepth(float2 UV)
            {
                return PointPixelate(_CameraDepthTexture, UV);
            }

            float Outline(float2 UV, float DepthThreshold, float NormalsThreshold, float3 NormalEdgeBias, float DepthEdgeStrength, float NormalEdgeStrength)
            {
                float depth = GetDepth(UV);
                float3 normal = GetNormals(UV);

                float2 uvs[4];

                uvs[0] = UV + float2(0.0, _BlockSize.y);
                uvs[1] = UV - float2(0.0, _BlockSize.y);
                uvs[2] = UV + float2(_BlockSize.x, 0.0);
                uvs[3] = UV - float2(_BlockSize.x, 0.0);

                // Depth Edges
                float depths[4];
                float depthDifference = 0.0;
                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    depths[i] = GetDepth(uvs[i]);
                    
                    // for outlines on the edge of the object, use depth - depths[i]
                    // for outlines around the object, use depths[i] - depth
                    depthDifference += depth - depths[i];
                }
                float depthEdge = step(DepthThreshold, depthDifference);

                // Normal Edges
                float3 normals[4];
                float dotSum = 0.0;
                [unroll]
                for (int j = 0; j < 4; j++)
                {
                    normals[j] = GetNormals(uvs[j]);
                    float3 normalDiff = normal - normals[j];

                    float normalBiasDiff = dot(normalDiff, NormalEdgeBias);
                    float normalIndicator = smoothstep(-0.01, 0.01, normalBiasDiff);

                    dotSum += dot(normalDiff, normalDiff) * normalIndicator;
                }
                float indicator = sqrt(dotSum);
                float normalEdge = step(NormalsThreshold, indicator);

                return depthDifference < 0 ? 0 : (depthEdge > 0.0 ? (DepthEdgeStrength * depthEdge) : (NormalEdgeStrength * normalEdge));
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 color = PointPixelate(_CameraOpaqueTexture, input.texcoord);
                return color + Outline(input.texcoord, _DepthThreshold, _NormalsThreshold, _NormalEdgeBias, _DepthEdgeStrength, _NormalEdgeStrength);
            }
            ENDHLSL
        }
    }
}
