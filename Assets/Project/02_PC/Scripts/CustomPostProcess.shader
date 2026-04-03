Shader "CustomPostProcess"
{
    Properties
    {
        //外部から制御するためのプロパティ
        _InvertIntensity("Intensity",Range(0,1)) = 0
        _DistortionFreq("Distortion Frequency", Float) = 10
        _DistortionAmp("Distortion Amplitude", Float) = 0.01
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
            float _DistortionFreq;
            float _DistortionAmp;

            float2 _BumpCenter; //衝撃の中心座標(0～１)

            //フラグメントシェーダー(ピクセルごとの処理)
            half4 Frag (Varyings input) : SV_Target
            {
                //Unity 6/URPの作法：画面のテクスチャをサンプリング
                //_BlitTexture は Unity が自動的に現在の画面内容を割り当てる
                float2 uv = input.texcoord;


                // ===============
                // 波の歪みを発生
                // ===============

                // 波の歪みをつくる
                // float distortion = sin(uv.y * _DistortionFreq + _Time.y * 20.0) * _DistortionAmp;
                // 強度に合わせて歪みをかける
                // uv.x += distortion * _InvertIntensity;

                // 歪ませたUVでサンプリング
                // half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                // 元の色と反転した色を強度(_InvertIntensity)で線形補間する
                // float3 invertedColor = 1.0 - color.rgb;
                // float3 finalColor = lerp(color.rgb, invertedColor, _InvertIntensity);

                // 色を反転させる
                // return half4(finalColor, color.a);


                //=====================
                //衝突したら波紋を発生
                //=====================
                //中心からのベクトルと距離を計算
                float2 dir = uv - _BumpCenter;
                float dist = length(dir);

                //波紋の計算
                //距離と時間を使ってサイン波を作成
                float wave = sin(dist * 20.0 - _Time.y * 15.0);

                //衝撃の強さと距離の減衰を考慮してuvをずらす
                //中心に近いほど、かつ衝撃が強いほど大きく歪む
                float distortion = wave * _InvertIntensity * (1.0 - saturate(dist * 2.0));
                uv += normalize(dir) * distortion * 0.05;
                

                //====================================
                //グラフィックスAPIによって色彩を変更
                //====================================
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                float3 effectColor;

                #if defined(SHADER_API_D3D12)
                    //DX12
                    effectColor = float3(0.5, 0.8, 1.0) * color.rgb;
                #elif defined(SHADER_API_METAL)
                    //Metal
                    effectColor = float3(1.0, 0.6, 0.8) * color.rgb;
                #elif defined(SHADER_API_VULKAN)
                    //Vulkan
                    effectColor = float3(0.6, 1.0, 0.4) * color.rgb;
                #else
                    //その他は反転
                    effectColor = 1.0 - color.rgb;
                #endif

                //反転処理も強度に合わせる
                float3 finalColor = lerp(color.rgb, effectColor, _InvertIntensity);
        
                return half4(finalColor, color.a);
            }
            ENDHLSL
        }
    }
}