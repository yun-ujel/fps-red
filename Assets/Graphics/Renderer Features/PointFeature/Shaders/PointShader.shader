Shader "Screen/Point"
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

            float _RedThreshold;

            half4 frag (Varyings input) : SV_Target
            {
                int paletteSize = 3;

                half4 color = _CameraOpaqueTexture.Sample(sampler_point_clamp, input.texcoord);
                half joe = color;
                joe -= color.b;
                joe -= color.g;

                half4 red = half4(1, 0, 0, 1);
                half quantize = color = floor(color * (paletteSize - 1) + 0.5) / (paletteSize - 1);

                return joe > _RedThreshold ? red : quantize;
            }
            ENDHLSL
        }
    }
}
