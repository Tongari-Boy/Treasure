using UnityEngine;

public class CollisionEffectConstroller : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; //レイヤーをObstacleに設定したもののみを対象として扱う

    [SerializeField] private Material invertMaterial;
    [SerializeField] private float effectDuration = 0.5f;   //エフェクト時間
    private float _timer = 0f;

    void Update()
    {
        if(_timer > 0f)
        {
            _timer -= Time.deltaTime;

            // タイマーの残りに合わせて強度を下げる（0に近づく）
            float intensity = _timer / effectDuration;
            invertMaterial.SetFloat("_InvertIntensity", intensity);
        }
        else
        {
            invertMaterial.SetFloat("_InvertIntensity", 0f);
        }
    }

    //衝突時に呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            //衝突地点をワールド座標からスクリーン座標に変換
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(contactPoint);

            //シェーダーに中心点を送る
            invertMaterial.SetVector("_BumpCenter", new Vector4(screenPoint.x, screenPoint.y, 0, 0));

            _timer = effectDuration;
        }

        //指定したレイヤー(Obstacle)のオブジェクトにぶつかった時だけタイマーを動かす
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log("衝突");
            _timer = effectDuration;
        }
    }
}
