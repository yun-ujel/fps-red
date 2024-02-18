Shader "Screen/RedPalette"
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
            float4 _CameraOpaqueTexture_TexelSize;

            TEXTURE2D(_PaletteTexture);

            float _RedThreshold;
            int _PaletteSize;

            int _BayerLevel;
            float _DitherSpread;
            float _DitherScale;

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
                half4 color = _CameraOpaqueTexture.Sample(sampler_point_clamp, input.texcoord);
                
                half redValue = color;
                redValue -= color.b;
                redValue -= color.g;

                int x = input.texcoord.x * _CameraOpaqueTexture_TexelSize.z * _DitherScale;
                int y = input.texcoord.y * _CameraOpaqueTexture_TexelSize.w * _DitherScale;

                float bayerValues[3] = { 0, 0, 0 };
                bayerValues[0] = GetBayer2(x, y);
                bayerValues[1] = GetBayer4(x, y);
                bayerValues[2] = GetBayer8(x, y);

                half4 red = half4(1, 0, 0, 1);

                float4 output = color + _DitherSpread * bayerValues[_BayerLevel];

                half quantize = floor(output * (_PaletteSize - 1) + 0.5) / (_PaletteSize - 1);
                output = _PaletteTexture.Sample(sampler_point_clamp, quantize);

                return output;


            }
            ENDHLSL
        }
    }
}
