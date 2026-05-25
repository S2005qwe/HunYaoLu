using UnityEngine;
using UnityEngine.UI;

public class BellCraftView : MonoBehaviour
{
    [Header("进度条")]
    public Slider progressBar;

    [Header("文本")]
    public Text stateText;
    public Text countText;
    public Text timeText; // 剩余时间文本

    [Header("按钮")]
    public Button hitBtn;
    public Button restartBtn;
    public Button exitBtn;
    public Button startBtn; // 新增：开始按钮

    [Header("游戏元素")]
    public Image movingIcon;
    public Image targetArea;
    public float moveSpeed = 3f;
    public GameObject Jian;
    private RectTransform _barRect;
    private bool _isMovingRight = true;

    void Awake()
    {
        _barRect = progressBar.GetComponent<RectTransform>();
    }

    public void UpdateMoveIcon()
    {
        float barLeft = -_barRect.sizeDelta.x / 2;
        float barRight = _barRect.sizeDelta.x / 2;
        float currentX = movingIcon.rectTransform.anchoredPosition.x;

        if (_isMovingRight)
        {
            currentX += moveSpeed * Time.deltaTime * 100;
            if (currentX >= barRight) _isMovingRight = false;
        }
        else
        {
            currentX -= moveSpeed * Time.deltaTime * 100;
            if (currentX <= barLeft) _isMovingRight = true;
        }

        movingIcon.rectTransform.anchoredPosition = new Vector2(currentX, movingIcon.rectTransform.anchoredPosition.y);
    }

    public bool IsInTargetArea()
    {
        float iconX = movingIcon.rectTransform.anchoredPosition.x;
        RectTransform targetRect = targetArea.rectTransform;
        float center = targetRect.anchoredPosition.x;
        float halfW = targetRect.sizeDelta.x / 2;
        return iconX >= center - halfW && iconX <= center + halfW;
    }

    public void ResetIconPos()
    {
        float leftPos = -_barRect.sizeDelta.x / 2;
        movingIcon.rectTransform.anchoredPosition = new Vector2(leftPos, movingIcon.rectTransform.anchoredPosition.y);
        _isMovingRight = true;
    }

    // 显示剩余时间
    public void UpdateTimeDisplay(float time)
    {
        timeText.text = $"剩余时间{time:F1} 秒";
    }

    public void SetStateText(string text) => stateText.text = text;
    public void SetCountText(string text) => countText.text = text;
    public void SetHitButtonInteractable(bool value) => hitBtn.interactable = value;
    // 新增：控制开始按钮交互状态
    public void SetStartButtonInteractable(bool value) => startBtn.interactable = value;

    // 退出 = 隐藏当前UI界面
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
    // 显示箭
    public void ShowJian()
    {
        if (Jian != null)
        {
            Jian.SetActive(true);
        }
    }
}