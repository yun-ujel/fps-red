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

            TEXTURE2D(_PaletteTexture);

            int _PaletteSize;

            half4 frag (Varyings input) : SV_Target
            {
                half color = _CameraOpaqueTexture.Sample(sampler_point_clamp, input.texcoord);
                color = floor(color * (_PaletteSize - 1) + 0.5) / (_PaletteSize - 1);

                return _PaletteTexture.Sample(sampler_point_clamp, color);
            }
            ENDHLSL
        }
    }
}
