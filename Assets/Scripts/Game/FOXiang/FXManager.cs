using UnityEngine;
using System.Collections;

public class FXManager : Singleton<FXManager>
{
    public RotatingObject[] allObjects;  // 所有需要旋转的物体
    public GameObject[] otherObjects;
    public GameObject Drum;
    public GameObject Cube;
    public GameObject successEffect;     // 成功特效（可选）
    public AudioClip successSound;       // 成功音效（可选）
    public AudioClip rotateSound;        // 旋转音效（可选）
    [Header("平滑旋转速度")]
    public float rotateSpeed = 3f; // 调大=转得更快

    private bool isSolved = false;


    // 当任意物体被点击时调用
    public void OnObjectClicked(RotatingObject clickedObject)
    {
        if (isSolved) return;

        // 播放旋转音效
        if (rotateSound != null)
        {
            AudioSource.PlayClipAtPoint(rotateSound, Camera.main.transform.position);
        }

        // 旋转被点击的物体
        clickedObject.RotateOnce();

        // 检查是否解密完成
        CheckPuzzleComplete();
    }

    // 检查是否所有物体都到达目标角度
    private void CheckPuzzleComplete()
    {
        bool allCorrect = true;

        foreach (RotatingObject obj in allObjects)
        {
            if (!obj.IsCorrect())
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect && !isSolved)
        {
            Debug.Log("🎉 解密成功！所有物体都到达了目标角度！");
            isSolved = true;

            // 播放成功音效
            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);
            }

            EventHandler.CallTaskStatusUpdateEvent(3, TaskStatus.Completed);
            // 平滑旋转所有物体 + 触发后续事件
            StartCoroutine(ResetAllObjectsSmoothly());
        }
        else
        {
            // 输出当前进度（可选）
            PrintCurrentStatus();
        }
    }

    // 平滑旋转所有物体到目标角度（协程）
    private IEnumerator ResetAllObjectsSmoothly()
    {
        Debug.Log("🔄 开始平滑旋转所有物体到目标角度");
        Quaternion targetRot = Quaternion.Euler(0, 180, 0);
        Quaternion othertargetRot = Quaternion.Euler(0, 0, 0); ;

        // 平滑旋转 allObjects
        foreach (RotatingObject obj in allObjects)
        {
            obj.StartRotateToTarget(targetRot, rotateSpeed);
        }

        // 等待所有物体旋转完成
        bool rotating = true;
        while (rotating)
        {
            rotating = false;
            foreach (RotatingObject obj in allObjects)
            {
                if (obj.IsRotating)
                {
                    rotating = true;
                    break;
                }
            }
            yield return null;
        }

        // 等待一小段时间再旋转其他物体（视觉更舒服）
        yield return new WaitForSeconds(0.2f);

        // 平滑旋转 otherObjects
        foreach (GameObject obj in otherObjects)
        {
            StartCoroutine(RotateOtherObjectSmoothly(obj, othertargetRot));
        }

        // 等待其他物体旋转完成
        yield return new WaitForSeconds(1f);

        // 全部旋转完成后触发成功事件
        OnPuzzleSuccess();
    }

    // 平滑旋转 OtherObject
    private IEnumerator RotateOtherObjectSmoothly(GameObject obj, Quaternion targetRot)
    {
        while (Quaternion.Angle(obj.transform.rotation, targetRot) > 1f)
        {
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        obj.transform.rotation = targetRot;
    }

    // 解密成功后的处理
    private void OnPuzzleSuccess()
    {
        Drum.SetActive(true);
        //Cube.SetActive(false);
        Debug.Log("🔓 谜题完成！可以开门或触发其他事件");

        // 播放特效
        if (successEffect != null)
        {
            Instantiate(successEffect, Vector3.zero, Quaternion.identity);
        }

        //EventHandler.CallTaskStatusUpdateEvent(3, TaskStatus.Completed);
        TipsMgr.Instance.ShowSystemTips("解锁钟磬清韵");
        TipsMgr.Instance.ShowTaskTips("使用鼓震击钟磬");
    }

    // 输出当前所有物体的状态（用于调试）
    private void PrintCurrentStatus()
    {
        Debug.Log("=== 当前解密进度 ===");

        int correctCount = 0;
        for (int i = 0; i < allObjects.Length; i++)
        {
            RotatingObject obj = allObjects[i];
            int current = obj.GetCurrentYRotation();
            int target = obj.targetYRotation;

            if (current == target)
            {
                Debug.Log($"✅ {obj.name}：{current}° / 目标 {target}°");
                correctCount++;
            }
            else
            {
                Debug.Log($"❌ {obj.name}：{current}° / 目标 {target}°");
            }
        }

        Debug.Log($"进度：{correctCount}/{allObjects.Length}");
        Debug.Log("====================");
    }

    // 可选：按 I 键测试所有物体状态（不旋转，只检查）
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintCurrentStatus();
        }

        // 可选：按 R 键重置所有物体（作弊/调试用）
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("⚠️ 手动重置所有物体");
            foreach (RotatingObject obj in allObjects)
            {
                while (!obj.IsCorrect())
                {
                    obj.RotateOnce();
                }
            }
            CheckPuzzleComplete();
        }
    }
}