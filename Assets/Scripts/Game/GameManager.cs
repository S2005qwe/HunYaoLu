using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public List<GameOver> GameOvers;
    public GameObject Sphere; // 全部完成后显示的球体

    // ===================== 【计时器】 =====================
    [Header("游戏计时器")]
    public float currentGameTime = 0f;
    public bool isTimerRunning = true;

    public string FormattedTime => GetFormattedTime();

    // ===================== 【天空盒 + 渐变】 =====================
    [Header("天空盒设置")]
    public Material skyboxMaterial;

    [Header("全局渐变（Image + 自定义物体）")]
    public Image fadeImage;
    public Image overImage; // 最后显示的图片
    public float fadeDuration = 7f;
    public List<GameObject> fadeTargets;

    // ===================== 【碰撞后显示的物体】 =====================
    [Header("Sphere碰撞Jian后显示")]
    public List<GameObject> showAfterCollisionObjects; // 碰撞后显示的数组

    private bool allReadyShown = false;
    private bool isFadeComplete = false;
    private bool hasCollideJian = false; // 防止重复碰撞

    // ====================================================================

    private void Start()
    {
        LoadSkybox();
        StartFade();
    }

    private void Update()
    {
        // 1. 所有GameOver物体都显示 → 显示Sphere
        CheckAllGameOverActive();

        // 计时器
        if (isTimerRunning)
        {
            currentGameTime += Time.deltaTime;
        }

        // ===================== 【你要的逻辑：自动判断】 =====================
        if (overImage != null && UIManager.Instance != null)
        {
            // 如果 overImage 显示 → 关闭 MainCanvas
            if (overImage.gameObject.activeSelf)
            {
                UIManager.Instance.MainCanvas.SetActive(false);
            }
            // 否则 → 打开 MainCanvas
            else
            {
                UIManager.Instance.MainCanvas.SetActive(true);
            }
        }
        // ====================================================================

        RefreshTaskState();
        PrintAllTaskStatus();
    }

    // ===================== 1. 全部GameOver物体显示 → 显示Sphere =====================
    private void CheckAllGameOverActive()
    {
        if (allReadyShown) return;
        if (GameOvers == null || GameOvers.Count == 0) return;

        // 👇 直接判断【所有任务是否真的完成】
        bool allTaskFinished = true;

        foreach (var item in GameOvers)
        {
            bool isFinish = TaskGameManager.Instance.IsTaskCompleted(item.Taskid);

            if (!isFinish)
            {
                allTaskFinished = false;
              
                break;
            }
        }

        // 只有所有任务都完成 → 才显示Sphere
        if (allTaskFinished)
        {
            allReadyShown = true;
            if (Sphere != null)
            {
                Sphere.SetActive(true);
              
            }
        }
    }

    // ===================== 3. 碰撞成功：显示数组物体 → 6秒后显示Image =====================
    public void OnSphereHitJian()
    {
        if (hasCollideJian) return;
        hasCollideJian = true;

       

        // 显示数组里所有物体
        foreach (var obj in showAfterCollisionObjects)
        {
            if (obj != null) obj.SetActive(true);
        }

        // 6秒后显示 overImage
        StartCoroutine(ShowOverImageAfterDelay(6f));
    }

    private IEnumerator ShowOverImageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (overImage != null)
        {
            overImage.gameObject.SetActive(true);
           
        }

        isTimerRunning = false;
    }

    // ===================== 天空盒 =====================
    private void LoadSkybox()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }

    // ===================== 渐变 =====================
    public void StartFade()
    {
        StartCoroutine(FadeAllCoroutine());
    }

    private IEnumerator FadeAllCoroutine()
    {
        float timer = 0;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1;
            fadeImage.color = c;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, timer / fadeDuration);

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = a;
                fadeImage.color = c;
            }

            foreach (var go in fadeTargets)
            {
                if (go == null) continue;

                Renderer r = go.GetComponent<Renderer>();
                if (r != null)
                {
                    foreach (var mat in r.materials)
                    {
                        Color mc = mat.color;
                        mc.a = a;
                        mat.color = mc;
                    }
                }

                Image img = go.GetComponent<Image>();
                if (img != null)
                {
                    Color ic = img.color;
                    ic.a = a;
                    img.color = ic;
                }

                Text txt = go.GetComponent<Text>();
                if (txt != null)
                {
                    Color tc = txt.color;
                    tc.a = a;
                    txt.color = tc;
                }
            }

            yield return null;
        }

        if (fadeImage != null)
            fadeImage.gameObject.SetActive(false);

        isFadeComplete = true;
    }

    // ===================== 计时器 =====================
    public void PauseTimer() => isTimerRunning = false;
    public void ResumeTimer() => isTimerRunning = true;
    public void ResetTimer() => currentGameTime = 0f;

    private string GetFormattedTime()
    {
        int hour = (int)(currentGameTime / 3600);
        int min = (int)(currentGameTime % 3600 / 60);
        int sec = (int)(currentGameTime % 60);
        return $"{hour:00}:{min:00}:{sec:00}";
    }

    private void RefreshTaskState()
    {
        if (GameOvers == null) return;

        foreach (var item in GameOvers)
        {
            if (item.FireObject == null) continue;

            bool isFinish = TaskGameManager.Instance.IsTaskCompleted(item.Taskid);

            // 强制控制：完成显示，未完成强制隐藏
            item.FireObject.SetActive(isFinish);
        }
    }
    // 打印所有任务状态（已完成/未完成）
    public void PrintAllTaskStatus()
    {
        if (GameOvers == null || GameOvers.Count == 0)
        {
          
            return;
        }

      

        int completeCount = 0;
        int totalCount = GameOvers.Count;

        foreach (var item in GameOvers)
        {
            if (item == null) continue;

            bool isFinish = TaskGameManager.Instance?.IsTaskCompleted(item.Taskid) ?? false;
            if (isFinish) completeCount++;

           
        }

        
    }

}