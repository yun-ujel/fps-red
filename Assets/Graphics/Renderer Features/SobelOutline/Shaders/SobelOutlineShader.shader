Shader "Screen/Sobel Outline" {
	SubShader {
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
		
		Pass {
			Name "ColorBlitPass"

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			#pragma vertex Vert
            #pragma fragment frag

			float _SampleRange;
			float4 _OutlineColour;

			float _DepthThreshold;
			float _DepthTightening;
			float _DepthOutlineOpacity;

			float _NormalsThreshold;
			float _NormalsTightening;
			float _NormalsOutlineOpacity;

            TEXTURE2D(_CameraOpaqueTexture);
            TEXTURE2D(_CameraDepthTexture);
            TEXTURE2D(_CameraNormalsTexture);

			SamplerState sampler_bilinear_clamp;
			SAMPLER(sampler_CameraOpaqueTexture);


			static float2 sobelSamplePoints[9] = {
				float2(-1, 1), float2(0, 1), float2(1, 1),
				float2(-1, 0), float2(0, 0), float2(1, 0),
				float2(-1, -1), float2(0, -1), float2(1, -1)
			};

			static float sobelXMatrix[9] = {
				1, 0, -1,
				2, 0, -2,
				1, 0, -1
			};

			static float sobelYMatrix[9] = {
				1, 2, 1,
				0, 0, 0,
				-1, -2, -1
			};

			float DepthOutline(float2 UV, float sampleRange, float threshold, float tightening, float outlineOpacity) {
				float2 sobel = 0;

				[unroll] for (int i = 0; i < 9; i++) {
		        float depth = _CameraDepthTexture.Sample(sampler_bilinear_clamp, UV + (sobelSamplePoints[i] * sampleRange));
					sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
				}
				
				float sobelLength = length(sobel);
				float outline = smoothstep(0, threshold, sobelLength);
				
				outline = pow(outline, tightening);


				return outline * outlineOpacity;
			}

			float NormalsOutline(float2 UV, float sampleRange, float threshold, float tightening, float outlineOpacity) {
				float2 sobelX = 0;
				float2 sobelY = 0;
				float2 sobelZ = 0;

				[unroll] for (int i = 0; i < 9; i++) {
				float3 normal = _CameraNormalsTexture.Sample(sampler_bilinear_clamp, UV + (sobelSamplePoints[i] * sampleRange));
					
					float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);

					sobelX += normal.x * kernel;
					sobelY += normal.y * kernel;
					sobelZ += normal.z * kernel;
				}

				float maxSobelLength = max(length(sobelX), max(length(sobelY), length(sobelZ)));

				float outline = smoothstep(0, threshold, maxSobelLength);

				outline = pow(outline, tightening);

				return outline * outlineOpacity;
			}

			half4 frag (Varyings input) : SV_Target {
			float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_bilinear_clamp, input.texcoord);
				float outline = max(DepthOutline(input.texcoord, _SampleRange, _DepthThreshold, _DepthTightening, _DepthOutlineOpacity), NormalsOutline(input.texcoord, _SampleRange, _NormalsThreshold, _NormalsTightening, _NormalsOutlineOpacity));

				return lerp(color, _OutlineColour, outline);
			}

			ENDHLSL
		}
	}
}