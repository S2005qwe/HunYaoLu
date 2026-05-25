using Unity.VisualScripting;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform vfxHitRed;
    //[SerializeField] private Transform vfxHitBigRed;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 100f;
        bulletRigidbody.velocity = transform.forward * speed;
    }
    private bool hasHit = false;
    private void OnTriggerEnter(Collider other)
    {

        if (hasHit) return;

        bool isBowTaskUnlocked = IsBowTaskUnlocked();

        if (other.CompareTag("Tong")&&isBowTaskUnlocked)
        {
            hasHit = true;

            TongDestroy tong = other.GetComponent<TongDestroy>();
            if (tong != null)
            {
                tong.DestroySelfAndNearby();
            }

            Destroy(gameObject);
            return;
        }
        if(other.CompareTag("Over"))
        {
            GameManager.Instance.OnSphereHitJian();
        }
        // ----------------------
        // 射到其他物体
        // ----------------------
        
            if (vfxHitRed != null&& isBowTaskUnlocked)
            {
                Transform spawnVfx = Instantiate(vfxHitRed, transform.position, transform.rotation);
                Destroy(spawnVfx.gameObject, 1f);
            }
        

        // 火箭销毁
        Destroy(gameObject);
    }

    private bool IsBowTaskUnlocked()
    {
        if (TaskGameManager.Instance == null)
            return false;

        TaskDetails task = TaskGameManager.Instance.GetTaskDetails(0);
        return task != null && task.taskStatus == TaskStatus.Completed;
    }
}