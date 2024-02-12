using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class PixelateFeature : ScriptableRendererFeature
    {
        [SerializeField] private bool runInSceneView;
        [SerializeField] private PixelatePassSettings settings;
        private PixelatePass pixelatePass;

        private Material material;

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Screen/Pixelate");
            pixelatePass = new PixelatePass(settings, material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            pixelatePass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            pixelatePass.SetTarget(renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!runInSceneView && renderingData.cameraData.isSceneViewCamera) { return; }
#endif
            renderer.EnqueuePass(pixelatePass);
        }
    }

}