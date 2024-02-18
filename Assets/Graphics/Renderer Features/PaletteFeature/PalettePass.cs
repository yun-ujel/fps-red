using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace fpsRed.Graphics.RendererFeatures
{
    public class PalettePass : ScriptableRenderPass
    {
        public PalettePass(PalettePassSettings settings, Material material)
        {
            this.settings = settings;
            renderPassEvent = settings.RenderPassEvent;

            this.material = material;
        }

        private PalettePassSettings settings;
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
            using (new ProfilingScope(cmd, new ProfilingSampler("Palette Pass")))
            {
                int screenHeight = settings.PixelScreenHeight;
                int screenWidth = Mathf.RoundToInt(renderingData.cameraData.camera.aspect * screenHeight);

                material.SetVector("_BlockCount", new Vector2(screenWidth, screenHeight));
                material.SetVector("_BlockSize", new Vector2(1.0f / screenWidth, 1.0f / screenHeight));
                material.SetVector("_HalfBlockSize", new Vector2(0.5f / screenWidth, 0.5f / screenHeight));

                material.SetInt("_RedColourCount", settings.RedColourCount);
                material.SetInt("_GreenColourCount", settings.GreenColourCount);
                material.SetInt("_BlueColourCount", settings.BlueColourCount);

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