Shader "Screen/Palette"
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
            TEXTURE2D(_CameraOpaqueTexture);

            float2 _BlockCount;
            float2 _BlockSize;
            float2 _HalfBlockSize;

            int _RedColourCount, _GreenColourCount, _BlueColourCount;
            
            int _BayerLevel;
            float _DitherSpread;            

            static const int bayer2[2 * 2] = {
                0, 2,
                3, 1
            };

            static const int bayer4[4 * 4] = {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 5
            };

            static const int bayer8[8 * 8] = {
                0, 32, 8, 40, 2, 34, 10, 42,
                48, 16, 56, 24, 50, 18, 58, 26,  
                12, 44,  4, 36, 14, 46,  6, 38, 
                60, 28, 52, 20, 62, 30, 54, 22,  
                3, 35, 11, 43,  1, 33,  9, 41,  
                51, 19, 59, 27, 49, 17, 57, 25, 
                15, 47,  7, 39, 13, 45,  5, 37, 
                63, 31, 55, 23, 61, 29, 53, 21
            };

            
            float GetBayer2(int x, int y) {
                return float(bayer2[(x % 2) + (y % 2) * 2]) * (1.0f / 4.0f) - 0.5f;
            }

            float GetBayer4(int x, int y) {
                return float(bayer4[(x % 4) + (y % 4) * 4]) * (1.0f / 16.0f) - 0.5f;
            }

            float GetBayer8(int x, int y) {
                return float(bayer8[(x % 8) + (y % 8) * 8]) * (1.0f / 64.0f) - 0.5f;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 blockPos = floor(input.texcoord * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                half4 color = _CameraOpaqueTexture.Sample(sampler_point_clamp, blockCenter);
                
                float bayerValues[3] = { 0, 0, 0 };
                bayerValues[0] = GetBayer2(blockPos.x, blockPos.y);
                bayerValues[1] = GetBayer4(blockPos.x, blockPos.y);
                bayerValues[2] = GetBayer8(blockPos.x, blockPos.y);

                float4 output = color + _DitherSpread * bayerValues[_BayerLevel];

                output.r = floor((_RedColourCount - 1.0f) * output.r + 0.5) / (_RedColourCount - 1.0f);
                output.g = floor((_GreenColourCount - 1.0f) * output.g + 0.5) / (_GreenColourCount - 1.0f);
                output.b = floor((_BlueColourCount - 1.0f) * output.b + 0.5) / (_BlueColourCount - 1.0f);

                return output;
            }
            ENDHLSL
        }
    }
}
