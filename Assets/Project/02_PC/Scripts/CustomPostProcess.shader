Shader "CustomPostProcess"
{
    Properties
    {
        //外部から制御するためのプロパティ
        _InvertIntensity("Intensity",Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "InvertPass"

            HLSLPROGRAM
            //各プラットフォーム（DX12/Vulkan/Metal）向けのコンパイル指定
            #pragma vertex Vert
            #pragma fragment Frag

            //URPの標準ライブラリをインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //Blit（画面転送）用の便利な関数群をインクルード
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            //データの構造体
            //Vert関数は Blit.hlsl 内に定義されている標準的なものを使用

            // C#側から送られてくる値を定義
            float _InvertIntensity;

            //フラグメントシェーダー(ピクセルごとの処理)
            half4 Frag (Varyings input) : SV_Target
            {
                //Unity 6/URPの作法：画面のテクスチャをサンプリング
                //_BlitTexture は Unity が自動的に現在の画面内容を割り当てる
                float2 uv = input.texcoord;
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                //元の色と反転した色を強度(_InvertIntensity)で線形補間する
                float3 invertedColor = 1.0 - color.rgb;
                float3 finalColor = lerp(color.rgb, invertedColor, _InvertIntensity);

                //色を反転させる
                return half4(finalColor, color.a);
            }
            ENDHLSL
        }
    }
}