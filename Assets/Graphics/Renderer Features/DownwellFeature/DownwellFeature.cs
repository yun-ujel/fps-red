using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class DownwellFeature : ScriptableRendererFeature
    {
        [SerializeField] private bool runInSceneView;
        [SerializeField] private DownwellPassSettings settings;
        private DownwellPass downwellPass;

        private Material material;

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Screen/Downwell");
            downwellPass = new DownwellPass(settings, material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            downwellPass.ConfigureInput(ScriptableRenderPassInput.Color);
            downwellPass.SetTarget(renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!runInSceneView && renderingData.cameraData.isSceneViewCamera) { return; }
#endif
            renderer.EnqueuePass(downwellPass);
        }
    }

}