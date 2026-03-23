using Unity.Netcode;
using UnityEngine;

/// <summary>
/// スマートフォン側でボタンを押すと
/// PC側のオブジェクトの色が変わる処理を施します。
/// PCとスマートフォンが繋がっているかを確認するためのスクリプトです。
/// </summary>

public class ColorSync : MonoBehaviour
{
    //ネットワーク全体で同期される変数
    private NetworkVariable<Color> _networkColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server // サーバー（ホスト）だけが値を確定できる
    );
    
    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

   void Update()
    {
        //変数が変わったら見た目に反映
        if(_meshRenderer != null)
        {
            _meshRenderer.material.color = _networkColor.Value;
        }
    }

    //[クライアント(スマホ)から呼ぶ命令]
    [Rpc(SendTo.Server)]
    public void ChangeColorServerRpc()
    {
        //ホスト側でランダムな色を生成して同期変数にセット
        _networkColor.Value = new Color(Random.value, Random.value, Random.value);
    }
}
