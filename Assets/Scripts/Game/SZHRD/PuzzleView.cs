using UnityEngine;
using System;

/// <summary>
/// MVC-V 每个Cube自己的视图
/// 存自身数字、矩阵坐标、检测点击、平滑移动
/// </summary>
public class PuzzleView : MonoBehaviour
{
    [Header("当前数字，空位设为9")]
    public int currentNumber;
    [Header("九宫格矩阵坐标 行,列")]
    public Vector2Int gridPos;

    [Header("平滑移动速度")]
    public float moveSpeed = 8f;

    private Vector3 _targetPos;
    public bool IsMoving { get; private set; }

    public Action<PuzzleView> OnClickEvent;

    private void Update()
    {
        if (!IsMoving) return;

        transform.position = Vector3.Lerp(transform.position, _targetPos, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPos) < 0.02f)
        {
            transform.position = _targetPos;
            IsMoving = false;
        }
    }


    private void OnMouseDown()
    {
        if (currentNumber == 9 || IsMoving) return;
        OnClickEvent?.Invoke(this);
    }


    public void MoveTo(Vector3 target)
    {
        _targetPos = target;
        IsMoving = true;
    }
}