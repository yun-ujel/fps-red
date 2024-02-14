using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class RedPaletteFeature : ScriptableRendererFeature
    {
        [SerializeField] private bool runInSceneView;
        [SerializeField] private RedPalettePassSettings settings;
        private RedPalettePass pixelatePass;

        private Material material;

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Screen/RedPalette");
            pixelatePass = new RedPalettePass(settings, material);
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