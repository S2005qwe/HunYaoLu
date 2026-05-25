using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("加载面板")]
    public GameObject loadingPanel;
    public Slider progressSlider;
    public Text progressText;

    [Header("渐变图片")]
    //public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("菜单面板")]
    public GameObject menuPanel;

    [Header("进度条速度")]
    public float fakeLoadSpeed = 0.5f;

    private void Start()
    {
        loadingPanel.SetActive(false);
        // 初始设为完全透明
        //Color c = fadeImage.color;
        //c.a = 0;
        //fadeImage.color = c;
    }

    public void TransScene()
    {
        Debug.Log("21");
        loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneWithProgress());
    }

    IEnumerator LoadSceneWithProgress()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("HTS", LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        float fakeProgress = 0;
        while (fakeProgress < 1)
        {
            fakeProgress += Time.deltaTime * fakeLoadSpeed;
            fakeProgress = Mathf.Min(fakeProgress, 1);
            progressSlider.value = fakeProgress;
            progressText.text = (fakeProgress * 100).ToString("0") + "%";
            yield return null;
        }

        progressSlider.value = 1;
        progressText.text = "100%";
        yield return new WaitForSeconds(0.2f);

        asyncOperation.allowSceneActivation = true;
        yield return asyncOperation;

        Destroy(menuPanel);
        yield return StartCoroutine(DoFadeOut());
        loadingPanel.SetActive(false);
    }

    IEnumerator DoFadeOut()
    {
        // 先拉满黑屏再淡出
        float timer = 0;
        //Color color = fadeImage.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
           // color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            //fadeImage.color = color;
            yield return null;
        }
       // color.a = 1;
        //fadeImage.color = color;

        // 再从1渐变回0
        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
          //  color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
           // fadeImage.color = color;
            yield return null;
        }
        //color.a = 0;
        //fadeImage.color = color;
    }
}