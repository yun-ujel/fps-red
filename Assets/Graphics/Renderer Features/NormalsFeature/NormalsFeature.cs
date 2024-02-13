using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class NormalsFeature : ScriptableRendererFeature
    {
        [SerializeField] private bool runInSceneView;
        [SerializeField] private NormalsPassSettings settings;
        private NormalsPass pixelatePass;

        private Material material;

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Screen/Normals");
            pixelatePass = new NormalsPass(settings, material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            pixelatePass.ConfigureInput(ScriptableRenderPassInput.Normal);
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