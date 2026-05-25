using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteSpawn : Singleton<NoteSpawn>
{
    [Header("基础设置")]
    public NoteData_SO musicTrackData;
    public GameObject notePrefab;
    public ButtonController buttonController;
    public float noteMoveSpeed = 100f;

    [Header("UI显示设置")]
    public GameObject targetUIPanel;
    public float delayShowPanel = 4f;
    [Header("面板显示多久后关闭")]
    public float showPanelDuration = 3f; // 新增：显示几秒后关闭

    private Dictionary<int, Image> laneImageMap = new Dictionary<int, Image>();
    private Coroutine spawnCoroutine;
    private bool isAllNotesSpawned = false;

    void Start()
    {
        InitLaneImageMap();
        musicTrackData?.SortAllNotes();

        if (targetUIPanel != null)
        {
            targetUIPanel.SetActive(false);
        }
    }

    void InitLaneImageMap()
    {
        if (buttonController == null) return;

        foreach (var btnInfo in buttonController.buttonInfos)
        {
            if (btnInfo != null && btnInfo.theImage != null && !laneImageMap.ContainsKey(btnInfo.TrackId))
            {
                laneImageMap.Add(btnInfo.TrackId, btnInfo.theImage);
            }
        }
    }

    /// <summary>
    /// 外部调用：开始生成音符（自动重置）
    /// </summary>
    public void StartSpawnNotes()
    {
        // 停止上一次协程
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // 清空所有旧音符
        ClearAllNotes();

        // 重置状态
        isAllNotesSpawned = false;
        if (targetUIPanel != null) targetUIPanel.SetActive(false);

        // 启动新协程
        spawnCoroutine = StartCoroutine(SpawnNotesCoroutine());
    }

    IEnumerator SpawnNotesCoroutine()
    {
        Dictionary<int, int> laneNoteIndex = new Dictionary<int, int>();
        foreach (var lane in musicTrackData.allLanes)
        {
            laneNoteIndex[lane.laneId] = 0;
        }

        float startTime = Time.time;

        while (true)
        {
            float currentTime = Time.time - startTime;

            foreach (var lane in musicTrackData.allLanes)
            {
                if (!laneImageMap.ContainsKey(lane.laneId)) continue;
                if (laneNoteIndex[lane.laneId] >= lane.laneNotes.Count) continue;

                DropNoteInfo noteInfo = lane.laneNotes[laneNoteIndex[lane.laneId]];

                if (currentTime >= noteInfo.fallStartTime)
                {
                    SpawnSingleNote(lane.laneId, noteInfo);
                    laneNoteIndex[lane.laneId]++;
                }
            }

            bool allNotesSpawned = CheckAllNotesSpawned(laneNoteIndex);

            if (allNotesSpawned && !isAllNotesSpawned)
            {
                isAllNotesSpawned = true;
                StartCoroutine(WaitAllNotesVanishThenShowPanel()); // 改这里
                break;
            }

            if (allNotesSpawned) break;

            yield return null;
        }
    }

    #region 独立函数
    /// <summary>
    /// 生成单个音符
    /// </summary>
    public void SpawnSingleNote(int laneId, DropNoteInfo noteInfo)
    {
        if (!laneImageMap.TryGetValue(laneId, out Image targetImage)) return;
        if (notePrefab == null) return;

        GameObject noteObj = Instantiate(notePrefab);
        RectTransform noteRect = noteObj.GetComponent<RectTransform>();
        RectTransform targetRect = targetImage.rectTransform;

        noteRect.SetParent(targetRect, false);
        noteRect.anchoredPosition = new Vector2(0, targetRect.sizeDelta.y / 2 + 1050);

        NoteObject note = noteObj.GetComponent<NoteObject>();
        if (note != null)
        {
            note.judgmentLineY = 0;
            note.beatTempo = noteMoveSpeed;
            note.noteImage = noteObj.GetComponent<Image>();
            note.StartScroll();
        }

        noteObj.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    /// <summary>
    /// 检查是否全部生成完毕
    /// </summary>
    private bool CheckAllNotesSpawned(Dictionary<int, int> laneNoteIndex)
    {
        foreach (var lane in musicTrackData.allLanes)
        {
            if (laneNoteIndex[lane.laneId] < lane.laneNotes.Count)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 清空场景中所有旧音符（关键）
    /// </summary>
    public void ClearAllNotes()
    {
        NoteObject[] notes = FindObjectsOfType<NoteObject>();
        foreach (var note in notes)
        {
            Destroy(note.gameObject);
        }
    }
    #endregion

    // 【你原有方法保留，我只是不用它了】
    IEnumerator DelayShowUIPanel()
    {
        yield return new WaitForSeconds(delayShowPanel);
        if (targetUIPanel != null) targetUIPanel.SetActive(true);
    }

    // ====================== 【新增：你要的逻辑】 ======================
    IEnumerator WaitAllNotesVanishThenShowPanel()
    {
        // 1. 等待所有音符消失
        while (FindObjectsOfType<NoteObject>().Length > 0)
        {
            yield return null;
        }

        // 2. 延迟delayShowPanel秒后显示面板
        yield return new WaitForSeconds(delayShowPanel);

        if (targetUIPanel != null)
        {
            targetUIPanel.SetActive(true);

            // 3. 显示 showPanelDuration 秒后自动关闭
            yield return new WaitForSeconds(showPanelDuration);
            MusicUIManager.Instance.MusicPanel.SetActive(false);
            MusicUIManager.Instance.MusicCamera.SetActive(false);
            UIPuzzlePanel.Instance.MainCanvas.SetActive(true);

            TipsMgr.Instance.ShowSystemTips("恭喜你！距离成功仅差最后一步");
            TipsMgr.Instance.ShowTaskTips("解决浑天仪异象");
            targetUIPanel.SetActive(false);
        }
    }
    // ================================================================

    public void StopSpawnNotes()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if (!isAllNotesSpawned)
        {
            isAllNotesSpawned = true;
            StartCoroutine(WaitAllNotesVanishThenShowPanel()); // 改这里
        }
    }
}