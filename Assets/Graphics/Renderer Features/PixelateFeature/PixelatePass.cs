using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class PixelatePass : ScriptableRenderPass
    {
        public PixelatePass(PixelatePassSettings settings, Material material)
        {
            this.settings = settings;
            renderPassEvent = settings.RenderPassEvent;

            this.material = material;
        }

        private PixelatePassSettings settings;
        private RTHandle cameraColorTarget;

        private int pixelScreenHeight;
        private int pixelScreenWidth;

        private Material material;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(cameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelate Pass")))
            {
                pixelScreenHeight = settings.ScreenHeight;
                pixelScreenWidth = Mathf.RoundToInt(renderingData.cameraData.camera.aspect * pixelScreenHeight);

                material.SetFloat("_UpscaleRatio", renderingData.cameraData.camera.pixelHeight / pixelScreenHeight);

                material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
                material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
                material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

                material.SetFloat("_DepthThreshold", settings.DepthThreshold);
                material.SetFloat("_NormalsThreshold", settings.NormalsThreshold);

                material.SetVector("_NormalEdgeBias", settings.NormalEdgeBias);

                material.SetFloat("_DepthEdgeStrength", settings.DepthEdgeStrength);
                material.SetFloat("_NormalEdgeStrength", settings.NormalEdgeStrength);

                Blitter.BlitCameraTexture(cmd, cameraColorTarget, cameraColorTarget, material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public void SetTarget(RTHandle target)
        {
            cameraColorTarget = target;
        }
    }
}