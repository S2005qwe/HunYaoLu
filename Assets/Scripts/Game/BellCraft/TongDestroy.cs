using UnityEngine;
using System.Collections;

public class TongDestroy : MonoBehaviour
{
    [Header("特效")]
    [SerializeField] private Transform vfxHitBigRed;

    [Header("爆炸参数")]
    [SerializeField] private float explodeRadius = 5f;
    [SerializeField] private float explosionDelay = 0.1f;

    private bool isDestroyed = false;

    #region 公共方法

    /// <summary>
    /// 第一个被射中的 Tong 立即爆炸（无延迟）
    /// </summary>
    public void DestroySelfAndNearby()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        ImmediateExplosion();
    }

    /// <summary>
    /// 链式爆炸（延迟触发）
    /// </summary>
    public void TriggerChainExplosion()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        StartCoroutine(DelayedExplosion());
    }

    #endregion

    #region 爆炸逻辑

    /// <summary>
    /// 立即爆炸（用于第一个 Tong）
    /// </summary>
    private void ImmediateExplosion()
    {
        if (this == null || gameObject == null) return;

        ExplodeAndSpread();
        Destroy(gameObject);
    }

    /// <summary>
    /// 延迟爆炸协程（用于连锁的 Tong）
    /// </summary>
    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (this == null || gameObject == null) yield break;

        ExplodeAndSpread();
        Destroy(gameObject);
    }

    private void ExplodeAndSpread()
    {
        SpawnEffect(transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRadius);
        Debug.Log($"共检测到 {colliders.Length} 个碰撞体");

        foreach (Collider nearby in colliders)
        {
            Debug.Log($"物体: {nearby.name}, 标签: {nearby.tag}, 位置: {nearby.transform.position}");

            // 处理 Tong 的链式爆炸
            if (nearby.CompareTag("Tong"))
            {
                TongDestroy nearbyTong = nearby.GetComponent<TongDestroy>();
                if (nearbyTong != null)
                {
                    nearbyTong.TriggerChainExplosion();
                }
            }

            // 处理 FirstDoor 的隐藏
            if (nearby.CompareTag("FirstDoor"))
            {
                Debug.Log($"✓✓✓ 成功找到 FirstDoor: {nearby.name} ✓✓✓");

                // 方案1：隐藏碰撞体所在的 GameObject（当前对象）
                nearby.transform.parent.gameObject.SetActive(false);
                TipsMgr.Instance.ShowSystemTips("恭喜你！现在开始自由探索吧");
                TipsMgr.Instance.ShowTaskTips("自由探索，或寻找书籍线索");

            }
        }
    }

    #endregion

    #region 特效

    /// <summary>
    /// 生成爆炸特效
    /// </summary>
    private void SpawnEffect(Vector3 position)
    {
        if (vfxHitBigRed == null) return;

        Transform vfx = Instantiate(vfxHitBigRed, position, Quaternion.identity);
        Destroy(vfx.gameObject, 1f);
    }

    #endregion

    #region 调试

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }

    #endregion
}