using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class RedPalettePassSettings
    {
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;
        [field: SerializeField, Range(0f, 1f)] public float RedThreshold { get; set; }
        [field: Header("Palette"), SerializeField, Range(2, 32)] public int PaletteSize { get; set; }
        [field: SerializeField] public Texture2D Palette { get; set; }
    }
}