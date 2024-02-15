using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutlineFeature : ScriptableRendererFeature
{
    private SobelOutlinePass sobelOutlinePass;

    private Material material;
    [SerializeField] private SobelOutlinePassSettings settings;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial("Screen/Sobel Outline");
        sobelOutlinePass = new SobelOutlinePass(settings, material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        sobelOutlinePass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        sobelOutlinePass.SetTarget(renderer.cameraColorTargetHandle);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(sobelOutlinePass);
    }
}
