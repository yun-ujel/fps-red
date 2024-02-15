using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutlinePass : ScriptableRenderPass
{
    public SobelOutlinePass(SobelOutlinePassSettings settings, Material material)
    {
        renderPassEvent = settings.RenderPassEvent;

        this.settings = settings;
        this.material = material;
    }

    private SobelOutlinePassSettings settings;
    private Material material;
    private RTHandle cameraColorTarget;

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
        using (new ProfilingScope(cmd, new ProfilingSampler("Sobel Outline Pass")))
        {
            material.SetFloat("_SampleRange", settings.SampleRange);
            material.SetColor("_OutlineColour", settings.OutlineColour);

            material.SetFloat("_DepthThreshold", settings.DepthThreshold);
            material.SetFloat("_DepthTightening", settings.DepthTightening);
            material.SetFloat("_DepthOutlineOpacity", settings.DepthOutlineOpacity);

            material.SetFloat("_NormalsThreshold", settings.NormalsThreshold);
            material.SetFloat("_NormalsTightening", settings.NormalsTightening);
            material.SetFloat("_NormalsOutlineOpacity", settings.NormalsOutlineOpacity);


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

[System.Serializable]
public struct SobelOutlinePassSettings
{
    [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; }

    [field: SerializeField, Range(0, 0.1f)] public float SampleRange { get; set; }
    [field: SerializeField] public Color OutlineColour { get; set; }

    [field: Header("Depth Outlines"), SerializeField] public float DepthThreshold { get; set; }
    [field: SerializeField] public float DepthTightening { get; set; }
    [field: SerializeField, Range(0, 1)] public float DepthOutlineOpacity { get; set; }

    [field: Header("Normals Outlines"), SerializeField] public float NormalsThreshold { get; set; }
    [field: SerializeField] public float NormalsTightening { get; set; }
    [field: SerializeField, Range(0, 1)] public float NormalsOutlineOpacity { get; set; }

    public SobelOutlinePassSettings(RenderPassEvent renderPassEvent, float sampleRange, Color outlineColour, float depthThreshold, float depthTightening, float depthOutlineOpacity, float normalsThreshold, float normalsTightening, float normalsOutlineOpacity)
    {
        RenderPassEvent = renderPassEvent;

        SampleRange = sampleRange;
        OutlineColour = outlineColour;

        DepthThreshold = depthThreshold;
        DepthTightening = depthTightening;
        DepthOutlineOpacity = depthOutlineOpacity;

        NormalsThreshold = normalsThreshold;
        NormalsTightening = normalsTightening;
        NormalsOutlineOpacity = normalsOutlineOpacity;
    }
}
