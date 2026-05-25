using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [Header("等级分数")]
    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    [Header("统计")]
    public float totalNotes;
    public float normalHits;
    public float goodHits;
    public float perfectHits;
    public float missedHits;

    void Start()
    {
        // 移到开始游戏时统计，避免Start提前统计空场景
    }

    private void OnMouseDown()
    {
        bool taskIsConpelet = TaskGameManager.Instance.IsTaskCompleted(3);
        if (MusicUIManager.Instance != null && taskIsConpelet&& UIPuzzlePanel.Instance != null)
        {
            ResetGameState();
            MusicUIManager.Instance.MusicPanel.SetActive(true);
            MusicUIManager.Instance.MusicCamera.SetActive(true);
            Debug.Log("sjb");
            UIPuzzlePanel.Instance.MainCanvas.SetActive(false);

            NoteSpawn.Instance.StartSpawnNotes();
        }
    }

    public void SwitchMainCanvasState(bool showMainCanvas)
    {
        if (UIPuzzlePanel.Instance == null)
            return;

        UIPuzzlePanel.Instance.MainCanvas.SetActive(showMainCanvas);
    }

    public void StartGame()
    {
        ResetGameState();
    }

    public void NormalHit()
    {
        currentScore += scorePerNote;
        normalHits++;
        MusicUIManager.Instance.UpdateScoreDisplay(currentScore);
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote;
        goodHits++;
        MusicUIManager.Instance.UpdateScoreDisplay(currentScore);
    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote;
        perfectHits++;
        MusicUIManager.Instance.UpdateScoreDisplay(currentScore);
    }

    public void NoteMissed()
    {
        missedHits++;
    }

    public void ResetGameState()
    {
        currentScore = 0;
        normalHits = 0;
        goodHits = 0;
        perfectHits = 0;
        missedHits = 0;

        // 重新统计当前音符数量
        totalNotes = FindObjectsOfType<NoteObject>().Length;

        if (MusicUIManager.Instance != null)
            MusicUIManager.Instance.ResetUI();
    }
   
}