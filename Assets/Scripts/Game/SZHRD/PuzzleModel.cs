/// <summary>
/// MVC-M 纯数据层
/// 只存标准答案、游戏状态，不做逻辑、不找物体
/// </summary>
public class PuzzleModel
{
    // 正确排布 1~8 最后9为空位
    public readonly int[,] TargetGrid =
    {
        {1,2,3},
        {4,5,6},
        {7,8,9}
    };

    // 自定义初始打乱布局
    public readonly int[,] InitialShuffleGrid =
    {
        {6,8,4},
        {3,5,7},
        {1,2,9}
    };

    public bool IsGameWin { get; set; }
}