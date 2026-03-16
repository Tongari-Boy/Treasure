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

            //タイマーが動いている間は強度を1にする
            invertMaterial.SetFloat("_InvertIntensity", 1f);
        }
        else
        {
            invertMaterial.SetFloat("_InvertIntensity", 0f);
        }
    }

    //衝突時に呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        //指定したレイヤー(Obstacle)のオブジェクトにぶつかった時だけタイマーを動かす
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log("衝突");
            _timer = effectDuration;
        }
    }
}
