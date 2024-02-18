using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class PalettePassSettings
    {
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;


        [field: Header("Pixelation"), SerializeField] public int PixelScreenHeight;

        [field: Header("Palette"), SerializeField] public int RedColourCount { get; set; }
        [field: SerializeField] public int GreenColourCount { get; set; }
        [field: SerializeField] public int BlueColourCount { get; set; }

        [field: Header("Dithering"), SerializeField] public float DitherSpread { get; set; }
        [field: SerializeField, Range(0, 2)] public int BayerLevel { get; set; }
    }
}