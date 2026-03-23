using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host (PC側)")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client (Smartphone側)")) NetworkManager.Singleton.StartClient();
        }

        //接続中かつクライアント(スマホ)ならボタンを表示
        if (NetworkManager.Singleton.IsClient)
        {
            if (GUILayout.Button("色変更", GUILayout.Height(100)))
            {
                //シーン内のColorSyncを探してRPCを呼ぶ
                var sync = Object.FindFirstObjectByType<ColorSync>();
                if (sync != null) sync.ChangeColorServerRpc();
            }
        }
        GUILayout.EndArea();
    }
}