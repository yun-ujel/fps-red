using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class DownwellPassSettings
    {
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;

        
        [field: Header("Pixelation"), SerializeField] public int PixelScreenHeight;
        
        [field: Header("Palette"), SerializeField] public int PaletteSize { get; set; }
        [field: SerializeField] public Texture2D Palette { get; set; }
        [field: SerializeField, Range(0f, 1f)] public float RedThreshold { get; set; }

        [field: Header("Dithering"), SerializeField] public float DitherSpread { get; set; }
        [field: SerializeField, Range(0, 2)] public int BayerLevel { get; set; }
    }
}