using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class DownwellPass : ScriptableRenderPass
    {
        public DownwellPass(DownwellPassSettings settings, Material material)
        {
            this.settings = settings;
            renderPassEvent = settings.RenderPassEvent;

            this.material = material;
        }

        private DownwellPassSettings settings;
        private RTHandle cameraColorTarget;

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
            using (new ProfilingScope(cmd, new ProfilingSampler("Downwell Pass")))
            {
                material.SetFloat("_RedThreshold", settings.RedThreshold);

                int screenHeight = settings.PixelScreenHeight;
                int screenWidth = Mathf.RoundToInt(renderingData.cameraData.camera.aspect * screenHeight);

                material.SetVector("_BlockCount", new Vector2(screenWidth, screenHeight));
                material.SetVector("_BlockSize", new Vector2(1.0f / screenWidth, 1.0f / screenHeight));
                material.SetVector("_HalfBlockSize", new Vector2(0.5f / screenWidth, 0.5f / screenHeight));

                material.SetTexture("_PaletteTexture", settings.Palette);
                material.SetInt("_PaletteSize", settings.PaletteSize);

                material.SetTexture("_RedPaletteTexture", settings.RedPalette);
                material.SetInt("_RedPaletteSize", settings.RedPaletteSize);

                material.SetFloat("_DitherSpread", settings.DitherSpread);
                material.SetInt("_BayerLevel", settings.BayerLevel);

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