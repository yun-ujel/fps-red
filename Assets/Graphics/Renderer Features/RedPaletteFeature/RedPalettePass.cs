using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class RedPalettePass : ScriptableRenderPass
    {
        public RedPalettePass(RedPalettePassSettings settings, Material material)
        {
            this.settings = settings;
            renderPassEvent = settings.RenderPassEvent;

            this.material = material;
        }

        private RedPalettePassSettings settings;
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
            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelate Pass")))
            {
                material.SetFloat("_RedThreshold", settings.RedThreshold);

                material.SetInt("_PaletteSize", settings.PaletteSize);
                material.SetTexture("_PaletteTexture", settings.Palette);

                material.SetFloat("_DitherSpread", settings.DitherSpread);
                material.SetInt("_BayerLevel", settings.BayerLevel);
                material.SetFloat("_DitherScale", settings.DitherScale);

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