using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class PointPassSettings
    {
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;
        [field: SerializeField, Range(0f, 1f)] public float RedThreshold { get; set; }
    }
}