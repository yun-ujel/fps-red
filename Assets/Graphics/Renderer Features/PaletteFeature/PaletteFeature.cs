using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class PaletteFeature : ScriptableRendererFeature
    {
        [SerializeField] private bool runInSceneView;
        [SerializeField] private PalettePassSettings settings;
        private PalettePass pixelatePass;

        private Material material;

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Screen/Palette");
            pixelatePass = new PalettePass(settings, material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            pixelatePass.ConfigureInput(ScriptableRenderPassInput.Color);
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