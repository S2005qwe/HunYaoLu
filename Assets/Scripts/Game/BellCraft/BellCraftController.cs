using UnityEngine;

public class BellCraftController : MonoBehaviour
{
    [Header("游戏设置")]
    public int totalHitCount = 5;
    public float totalGameTime = 10f; // 游戏总时长

    [Header("UI引用")]
    public BellCraftView view;

    private BellCraftModel _model;

    void Start()
    {
        view = GetComponent<BellCraftView>();
        _model = new BellCraftModel(totalHitCount, totalGameTime);
        view.ResetIconPos();
        view.SetStateText("点击开始按钮启动游戏"); // 初始提示
        view.SetHitButtonInteractable(false); // 初始禁用击打按钮
        UpdateAllDisplay();

        // 按钮事件绑定
        view.hitBtn.onClick.AddListener(OnHitClick);
        view.restartBtn.onClick.AddListener(OnRestartClick);
        view.exitBtn.onClick.AddListener(OnExitClick);
        view.startBtn.onClick.AddListener(OnStartClick); // 绑定开始按钮
    }

    void Update()
    {
        // 仅当游戏「已开始」且「未结束」时，执行移动/计时逻辑
        if (_model.IsGameStarted && !_model.IsGameOver)
        {
            view.UpdateMoveIcon();
            _model.UpdateTimer(Time.deltaTime);
            UpdateAllDisplay();

            if (_model.RemainingTime <= 0)
            {
                GameEnd();
            }
        }
    }

    // 新增：开始按钮点击事件
    public void OnStartClick()
    {
        if (_model.IsGameStarted || _model.IsGameOver) return;

        _model.StartGame(); // 启动游戏
        view.SetStartButtonInteractable(false); // 禁用开始按钮
        view.SetHitButtonInteractable(true); // 启用击打按钮
        view.SetStateText("游戏中..."); // 更新状态提示
    }

    public void OnHitClick()
    {
        if (_model.IsGameOver || !_model.IsGameStarted) return; // 未开始则不响应

        bool success = view.IsInTargetArea();
        _model.Hit(success);

        view.SetStateText(success ? "<color=green>成功！</color>" : "<color=red>失败！</color>");
        UpdateAllDisplay();

        if (_model.IsGameOver)
            GameEnd();
    }

    // 重新开始
    public void OnRestartClick()
    {
        _model.Reset();
        view.ResetIconPos();
        view.SetHitButtonInteractable(false); // 重启后先禁用击打按钮
        view.SetStartButtonInteractable(true); // 重启后启用开始按钮
        view.SetStateText("点击开始按钮启动游戏");
        UpdateAllDisplay();
    }

    // 退出 = 关闭UI界面，游戏结束
    public void OnExitClick()
    {
        view.HideUI();
    }

    void GameEnd()
    {
        view.SetHitButtonInteractable(false);
        view.SetStateText(_model.IsWin() ? "<color=green>挑战成功！</color>" : "<color=red>挑战失败！</color>");
        if (_model.IsWin())
        {
            EventHandler.CallTaskStatusUpdateEvent(0, TaskStatus.Completed);
            TipsMgr.Instance.ShowSystemTips("恭喜你 成功将弓箭升级");
            TipsMgr.Instance.ShowTaskTips("根据线索打开山门");
            Debug.Log("成功");
            view.ShowJian();
        }
    }

    void UpdateAllDisplay()
    {
        view.SetCountText($"成功数{_model.SuccessCount} / 击打数{_model.CurrentHitCount}/{_model.TotalHitCount}");
        view.UpdateTimeDisplay(_model.RemainingTime);
    }
}