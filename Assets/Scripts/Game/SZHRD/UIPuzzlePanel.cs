using UnityEngine;
using UnityEngine.UI;

public class UIPuzzlePanel : Singleton<UIPuzzlePanel>
{
    public Button btnStart;
    public Button btnRestart;
    public GameObject Plane;
    public GameObject MainCanvas;
    public Button btnCamera;
    public Text stepText;

    protected override void Awake()
    {
        base.Awake();
        btnCamera.onClick.AddListener(OnHideCameraAndUI);
        btnStart.onClick.AddListener(OnStartGame);
        btnRestart.onClick.AddListener(OnRestartGame);
    }

    private void OnHideCameraAndUI()
    {
        if (PuzzleController.Instance?.CameraTarget != null)
            EventHandler.CallHidePuzzleUIEvent();
    }

    private void OnStartGame()
    {
        PuzzleController.Instance.StartGame();
        UpdateStepDisplay(0);
    }

    private void OnRestartGame()
    {
        PuzzleController.Instance.RestartGame();
        UpdateStepDisplay(0);
    }

    private void ShowUI()
    {
        Plane.SetActive(true);
        MainCanvas.SetActive(false);

        if (PuzzleController.Instance != null)
        {
            UpdateStepDisplay(PuzzleController.Instance.CurrentSteps);
        }
    }

    private void HideUI()
    {
        PuzzleController.Instance.CameraTarget.SetActive(false);
        Plane.SetActive(false);
        MainCanvas.SetActive(true);
    }

    // 🔥 公开方法，给控制器直接调用
    public void UpdateStepDisplay(int steps)
    {
        if (stepText != null)
        {
            stepText.text = "步数: " + steps;
        }
    }

    private void OnEnable()
    {
        EventHandler.ShowPuzzleUIEvent += ShowUI;
        EventHandler.HidePuzzleUIEvent += HideUI;

        // 🔥 删除步数订阅，全部清空
    }

    private void OnDisable()
    {
        EventHandler.ShowPuzzleUIEvent -= ShowUI;
        EventHandler.HidePuzzleUIEvent -= HideUI;

        // 🔥 删除取消订阅
    }
}