using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class PalettePassSettings
    {
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;
        [field: Space, SerializeField, Range(2, 16)] public int PaletteSize { get; set; }
        [field: SerializeField] public Texture2D Palette { get; set; }
    }
}