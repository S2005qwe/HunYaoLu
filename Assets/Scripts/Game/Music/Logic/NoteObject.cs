using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public float judgmentLineY;
    public GameObject hitEffect, goodEffect, perfectEffect;
    public Image noteImage;

    // 跟 BeatScroller 完全一样的速度计算
    public float beatTempo;
    private RectTransform rectTransform;

    private bool isMoving = true;
    private bool isMissed = false;

    void Start()
    {
        if (noteImage == null)
            noteImage = GetComponent<Image>();

        rectTransform = GetComponent<RectTransform>();

        // 完全照搬 BeatScroller 的计算
        beatTempo = beatTempo / 60f;
    }

    void Update()
    {
        // 【和 BeatScroller 一模一样的匀速运动】
        if (hasStarted && isMoving)
        {
            rectTransform.anchoredPosition -= new Vector2(0f, beatTempo * Time.deltaTime);
        }
    }

    // 外部控制是否开始滚动（和游戏同步开始）
    public bool hasStarted;
    public void StartScroll()
    {
        hasStarted = true;
    }

    public void Press()
    {
        if (!canBePressed || isMissed) return;

        isMoving = false;
        gameObject.SetActive(false);

        float distance = Mathf.Abs(rectTransform.anchoredPosition.y - judgmentLineY);

        if (distance > 25f)
        {
            MusicManager.Instance.NormalHit();
            SpawnUIEffect(hitEffect);
        }
        else if (distance > 10f)
        {
            MusicManager.Instance.GoodHit();
            SpawnUIEffect(goodEffect);
        }
        else
        {
            MusicManager.Instance.PerfectHit();
            SpawnUIEffect(perfectEffect);
        }
    }

    void SpawnUIEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null) return;

        GameObject effect = Instantiate(effectPrefab, MusicUIManager.Instance.EffectSpwn.transform.position, MusicUIManager.Instance.EffectSpwn.transform.rotation);
        effect.transform.SetParent(noteImage.canvas.transform, true);

        float destroyTime = 1f;
        if (effect.TryGetComponent<ParticleSystem>(out var ps))
            destroyTime = ps.main.duration;

        Destroy(effect, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            Debug.Log("1111");
            canBePressed = true;
            isMissed = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = false;

            // 没打到 → 算Miss → 继续匀速 → 2秒后销毁
            if (gameObject.activeSelf && !isMissed)
            {
                isMissed = true;
                MusicManager.Instance.NoteMissed();
                StartCoroutine(DelayDestroy(2f));
            }
        }
    }

    IEnumerator DelayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}