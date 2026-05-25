using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class PuzzleController : Singleton<PuzzleController>
{
    public GameObject CameraTarget;
    public GameObject Cube;
    public List<PuzzleView> cubeViews;
    private Vector3[] _fixedGrids = new Vector3[9];
    public PuzzleModel _model;

    private bool _isGameStarted;

    // 步数统计（去掉事件，改用直接调用）
    private int _currentSteps;
    public int CurrentSteps
    {
        get => _currentSteps;
        private set
        {
            _currentSteps = value;
            Debug.Log("当前步数: " + _currentSteps);

            // 🔥 直接调用 UI 更新，不用事件
            if (UIPuzzlePanel.Instance != null)
            {
                UIPuzzlePanel.Instance.UpdateStepDisplay(_currentSteps);
            }
        }
    }

    // 🔥 删除这行：public Action<int> OnStepChanged;

    protected override void Awake()
    {
        base.Awake();
        _model = new PuzzleModel();
    }

    private void Start()
    {
        for (int i = 0; i < 9; i++)
            _fixedGrids[i] = cubeViews[i].transform.position;

        foreach (var v in cubeViews)
            v.OnClickEvent = OnCubeClicked;

        CameraTarget.SetActive(false);
        CurrentSteps = 0;
    }

    private void OnMouseDown()
    {
        CameraTarget.SetActive(true);
        Debug.Log("显示拼图UI");
        EventHandler.CallShowPuzzleUIEvent();
    }

    public void StartGame()
    {
        _isGameStarted = true;
        CurrentSteps = 0;

        ShuffleCubesBetweenGrids();
        EventHandler.CallPuzzleStartedEvent();
    }

    public void RestartGame()
    {
        _isGameStarted = true;
        CurrentSteps = 0;

        _model.IsGameWin = false;
        ShuffleCubesBetweenGrids();
        EventHandler.CallPuzzleRestartedEvent();
    }

    private void ShuffleCubesBetweenGrids()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int targetNumber = _model.InitialShuffleGrid[row, col];
                PuzzleView cube = cubeViews.First(x => x.currentNumber == targetNumber);
                int gridIndex = GridToIndex(new Vector2Int(row, col));
                Vector3 targetPos = _fixedGrids[gridIndex];
                cube.MoveTo(targetPos);
                cube.gridPos = new Vector2Int(row, col);
            }
        }
        _model.IsGameWin = false;
    }

    public void OnCubeClicked(PuzzleView clicked)
    {
        if (!_isGameStarted) return;
        if (_model.IsGameWin || clicked.IsMoving) return;

        var empty = cubeViews.First(x => x.currentNumber == 9);
        if (!IsAdjacent(clicked, empty)) return;

        int clickedIndex = GridToIndex(clicked.gridPos);
        int emptyIndex = GridToIndex(empty.gridPos);

        Vector3 clickedTargetPos = _fixedGrids[clickedIndex];
        Vector3 emptyTargetPos = _fixedGrids[emptyIndex];

        clicked.MoveTo(emptyTargetPos);
        empty.MoveTo(clickedTargetPos);

        Vector2Int tempGrid = clicked.gridPos;
        clicked.gridPos = empty.gridPos;
        empty.gridPos = tempGrid;

        // 步数+1 → 会自动调用UI更新
        CurrentSteps++;

        CheckWin();
    }

    private bool IsAdjacent(PuzzleView a, PuzzleView b)
    {
        int dx = Mathf.Abs(a.gridPos.x - b.gridPos.x);
        int dy = Mathf.Abs(a.gridPos.y - b.gridPos.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private void CheckWin()
    {
        foreach (var cube in cubeViews)
        {
            int rightNum = _model.TargetGrid[cube.gridPos.x, cube.gridPos.y];
            if (cube.currentNumber != rightNum)
                return;
        }

        _model.IsGameWin = true;
        _isGameStarted = false;

        EventHandler.CallTaskStatusUpdateEvent(2, TaskStatus.Completed);
        Cube.SetActive(false);

        StartCoroutine(DelayedHideUI(2f));
        TipsMgr.Instance.ShowSystemTips("前面迷雾似乎消散了");
        TipsMgr.Instance.ShowTaskTips("点击佛像改变方向");
        Debug.Log($"拼图胜利！共走了 {CurrentSteps} 步");
    }

    private IEnumerator DelayedHideUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        EventHandler.CallHidePuzzleUIEvent();
    }

    private Vector2Int IndexToGrid(int index) => new Vector2Int(index / 3, index % 3);
    private int GridToIndex(Vector2Int grid) => grid.x * 3 + grid.y;
}