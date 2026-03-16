using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

//パスで使用するリソースの宣言
public class CustomRenderPassData
{
    public TextureHandle inputTexture;
}

//パスを実装する
public class CustomPass : ScriptableRenderPass
{
    private Material _material;

    public CustomPass(Material material)
    {
        _material = material; 

        //描画のタイミングを指定(ポストエフェクト)
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    //RecordRenderGraph
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        //カメラのターゲットが不正な場合は処理しない
        if (resourceData.isActiveTargetBackBuffer) return;

        //現在の画面（入力元）を取得
        TextureHandle source = resourceData.activeColorTexture;

        //一時的なテクスチャ（書き込み先）の設定と作成
        RenderTextureDescriptor cameraDesc = cameraData.cameraTargetDescriptor;
        TextureDesc desc = new TextureDesc(cameraDesc.width, cameraDesc.height);
        desc.format = cameraDesc.graphicsFormat;
        desc.name = "CustomPostProcessTemp";

        //予約:新しいメモリを確保
        TextureHandle destination = renderGraph.CreateTexture(desc);

        //レンダリングパスの構築
        using (var builder = renderGraph.AddRasterRenderPass<CustomRenderPassData>("CustomPass", out var passData))
        {
            //パスデータにリソースを渡す
            passData.inputTexture = source;

            //読み書きの対象を明確に分ける
            builder.UseTexture(source, AccessFlags.Read); //source は「読み」専用
            builder.SetRenderAttachment(destination, 0, AccessFlags.Write); //destination は「書き」専用

            builder.SetRenderFunc((CustomRenderPassData data, RasterGraphContext context) =>
            {
                //Shader側の _BlitTexture に data.inputTexture を割り当てて描画
                //描画先は自動的に builder.SetRenderAttachment で指定した destination になります
                Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), _material, 0);
            });
        }

        //次のパスでこの結果を使えるように、カメラの色情報を更新する
        resourceData.cameraColor = destination;
    }
}

//パスをUnityのレンダリングパイプラインに組み込む
public class CustomFeature : ScriptableRendererFeature
{
    public Material material;
    private CustomPass _pass;

    public override void Create()
    {
        _pass = new CustomPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}