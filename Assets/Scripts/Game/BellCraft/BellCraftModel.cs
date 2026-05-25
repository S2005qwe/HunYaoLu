public class BellCraftModel
{
    public int TotalHitCount { get; private set; }
    public int CurrentHitCount { get; private set; }
    public int SuccessCount { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsGameStarted { get; private set; } // 新增：游戏是否开始

    // 游戏时间
    public float TotalGameTime { get; private set; }
    public float RemainingTime { get; private set; }

    public BellCraftModel(int totalHitCount, float totalGameTime)
    {
        TotalHitCount = totalHitCount;
        TotalGameTime = totalGameTime;
        Reset();
    }

    // 新增：开始游戏
    public void StartGame()
    {
        IsGameStarted = true;
        RemainingTime = TotalGameTime; // 重置计时
    }

    public void Hit(bool isSuccess)
    {
        if (IsGameOver || !IsGameStarted) return; // 未开始则无法击打

        CurrentHitCount++;
        if (isSuccess) SuccessCount++;

        if (CurrentHitCount >= TotalHitCount)
            IsGameOver = true;
    }

    // 更新计时器
    public void UpdateTimer(float deltaTime)
    {
        if (IsGameOver || !IsGameStarted) return; // 未开始则不计时

        RemainingTime -= deltaTime;
        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            IsGameOver = true;
        }
    }

    public void Reset()
    {
        CurrentHitCount = 0;
        SuccessCount = 0;
        IsGameOver = false;
        IsGameStarted = false; // 重置为未开始
        RemainingTime = TotalGameTime;
    }

    // 胜利条件：成功次数≥3
    public bool IsWin() => SuccessCount >= 3;
}