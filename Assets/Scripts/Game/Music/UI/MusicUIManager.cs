using UnityEngine;
using UnityEngine.UI;

public class MusicUIManager : Singleton<MusicUIManager>
{
    [Header("文本")]
    public Text Title;
    public Text finalScoreText;
    public Text scoreText;
    public Text normalHitsText;
    public Text goodHitsText;
    public Text perfectHitsText;
    public Text missedHitsText;

    // 新增：倒计时文本
    public Text countdownText;

    [Header("组件引用")]
    public GameObject MusicPanel;
    public GameObject EffectSpwn;
    public GameObject MusicCamera;
  

    void Start()
    {
        ResetUI();

        // 初始化倒计时文本
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateHitDisplay();
    }

    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = "Score: " + score;
    }

    // 新增：更新倒计时显示
    public void UpdateCountdownDisplay(int countdownValue)
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdownValue.ToString();

            // 倒计时结束隐藏文本
            if (countdownValue <= 0)
            {
                countdownText.gameObject.SetActive(false);
            }
        }
    }

    void UpdateHitDisplay()
    {
        var mm = MusicManager.Instance;
        finalScoreText.text = mm.currentScore.ToString();
        normalHitsText.text = mm.normalHits.ToString();
        goodHitsText.text = mm.goodHits.ToString();
        perfectHitsText.text = mm.perfectHits.ToString();
        missedHitsText.text = mm.missedHits.ToString();
        Title.text = mm.currentScore > 1000 ? "挑战成功" : "挑战失败";
        if (mm.currentScore > 1000)
        {
            EventHandler.CallTaskStatusUpdateEvent(4,TaskStatus.Completed);
        }
    }

    //public void ShowResults()
    //{
    //    resultsScreen.SetActive(true);
    //}

    public void ResetUI()
    {
        scoreText.text = "Score: 0";
        //resultsScreen.SetActive(false);
        normalHitsText.text = "0";
        goodHitsText.text = "0";
        perfectHitsText.text = "0";
        missedHitsText.text = "0";

        // 重置倒计时文本
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }
}