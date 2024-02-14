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

            TEXTURE2D(_PaletteTexture);

            float _RedThreshold;
            int _PaletteSize;

            half4 frag (Varyings input) : SV_Target
            {
                half4 color = _CameraOpaqueTexture.Sample(sampler_point_clamp, input.texcoord);
                half joe = color;
                joe -= color.b;
                joe -= color.g;

                half4 red = half4(1, 0, 0, 1);
                half quantize = floor(color * (_PaletteSize - 1) + 0.5) / (_PaletteSize - 1);
                color = _PaletteTexture.Sample(sampler_point_clamp, color);

                return joe > _RedThreshold ? red : color;
            }
            ENDHLSL
        }
    }
}
