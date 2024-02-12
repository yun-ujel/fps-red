using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{

    [System.Serializable]
    public class PixelatePassSettings
    {
        [field: SerializeField] public int ScreenHeight { get; set; } = 360;
        [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;

        [field: Header("Outline Settings"), SerializeField]
        public float DepthThreshold { get; set; }
        [field: SerializeField] public float NormalsThreshold { get; set; }
        [field: SerializeField, Space] public Vector3 NormalEdgeBias { get; set; }
        [field: SerializeField, Space] public float DepthEdgeStrength { get; set; }
        [field: SerializeField] public float NormalEdgeStrength { get; set; }
    }
}