using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour
{
    [Header("目标角度（0, 90, 180, 270）")]
    public int targetYRotation = 0;  // 每个物体独立的目标角度

    private int currentYRotation = 0;  // 当前角度（0, 90, 180, 270）
    private int rotationCount = 0;  // 旋转次数（0=0°, 1=90°, 2=180°, 3=270°）
    private Coroutine rotateCoroutine;
    public bool IsRotating { get; private set; }

    private void Start()
    {
        currentYRotation = 0;
        rotationCount = 0;
    }

    // 旋转一次（增加90度）
    public void RotateOnce()
    {
        rotationCount++;
        if (rotationCount >= 4)
            rotationCount = 0;

        currentYRotation = rotationCount * 90;
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);

        Debug.Log($"🔄 {name} 旋转到 {currentYRotation}°");
    }

    // 检查是否达到目标角度
    public bool IsCorrect()
    {
        return currentYRotation == targetYRotation;
    }

    // 平滑旋转到目标角度（解密成功用）
    public void StartRotateToTarget(Quaternion targetRot, float speed)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(SmoothRotateTo(targetRot, speed));
    }

    private IEnumerator SmoothRotateTo(Quaternion targetRot, float speed)
    {
        IsRotating = true;
        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, speed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRot;
        currentYRotation = 0;
        rotationCount = 0;
        IsRotating = false;
    }

    // 获取当前角度
    public int GetCurrentYRotation()
    {
        return currentYRotation;
    }

    // 鼠标点击检测（挂在每个物体上）
    private void OnMouseDown()
    {
        FXManager manager = FindObjectOfType<FXManager>();
        bool taskIsConpelet = TaskGameManager.Instance.IsTaskCompleted(2);
        if (manager != null && taskIsConpelet)
        {
            manager.OnObjectClicked(this);
        }
    }
}