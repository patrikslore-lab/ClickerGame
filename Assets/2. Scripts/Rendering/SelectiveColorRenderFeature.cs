using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class SelectiveColorRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }

    public Settings settings = new Settings();
    private SelectiveColorRenderPass renderPass;
    private Material material;

    public override void Create()
    {
        if (settings.shader == null)
        {
            Debug.LogError("SelectiveColorRenderFeature: Shader is null");
            return;
        }

        material = CoreUtils.CreateEngineMaterial(settings.shader);
        renderPass = new SelectiveColorRenderPass(material, settings.renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null) return;

        // Update material with current color data from SelectiveColorManager
        if (SelectiveColorManager.Instance != null)
        {
            SelectiveColorManager.Instance.UpdateMaterialProperties(material);
        }

        renderer.EnqueuePass(renderPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }

    class SelectiveColorRenderPass : ScriptableRenderPass
    {
        private Material material;
        private const string profilerTag = "SelectiveColorPostProcess";

        public SelectiveColorRenderPass(Material material, RenderPassEvent renderPassEvent)
        {
            this.material = material;
            this.renderPassEvent = renderPassEvent;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            var source = resourceData.activeColorTexture;
            var descriptor = cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph, descriptor, "_TempSelectiveColorTexture", false);

            // Import the source texture so we can use it in the render function
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(profilerTag, out var passData))
            {
                passData.material = material;

                // Import the texture for reading
                builder.UseTexture(source, AccessFlags.Read);
                builder.SetRenderAttachment(destination, 0, AccessFlags.Write);

                // Set global texture that shader can access
                builder.SetGlobalTextureAfterPass(source, Shader.PropertyToID("_BlitTexture"));

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Use Blitter which properly handles the texture
                    Blitter.BlitTexture(context.cmd, source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // Copy back to source
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CopyBack", out var passData))
            {
                builder.UseTexture(destination, AccessFlags.Read);
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, destination, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }

        private class PassData
        {
            public Material material;
        }
    }
}
