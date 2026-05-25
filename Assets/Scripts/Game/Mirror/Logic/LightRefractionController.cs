using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 光线反射控制器 - MVC 控制层
/// </summary>
public class LightRefractionController : MonoBehaviour
{
    [Header("核心配置")]
    public Transform lightSource;
    public List<GameObject> reflectObjects = new List<GameObject>();
    public LightRefractionView view;

    [Header("淡出设置")]
    public float fadeDuration = 2f;

    private List<GameObject> currentHitOrder = new List<GameObject>();
    private bool isTaskCompleted = false;

    // ===================== 【新增：只弹一次提示】 =====================
    private bool hasShowTips = false;

    public bool IsAllObjectsActive()
    {
        if (reflectObjects.Count == 0) return false;
        foreach (var obj in reflectObjects)
        {
            if (!obj.activeInHierarchy)
                return false;
        }
        return true;
    }

    void Update()
    {
        UpdateReflectionLogic();
    }

    void UpdateReflectionLogic()
    {
        if (IsAllObjectsActive() && !isTaskCompleted)
        {
            // ===================== 【修复：只弹一次】 =====================
            if (!hasShowTips)
            {
                TipsMgr.Instance.ShowSystemTips("佛光显现，尽快完成任务");
                TipsMgr.Instance.ShowTaskTips("旋转铜镜台，完成正确反射");
                hasShowTips = true;
            }

            currentHitOrder.Clear();
            view.DrawReflectionRay(lightSource, reflectObjects, currentHitOrder);

            Transform parent = reflectObjects[0].transform.parent;
            Item item = parent.GetComponent<Item>();

            if (CheckReflectionOrderCorrect())
            {
                isTaskCompleted = true;
                EventHandler.CallTaskStatusUpdateEvent(1, TaskStatus.Completed);
                Debug.Log("顺序正确，光线开始淡出");
                StartCoroutine(FadeLightRoutine());
                EventHandler.CallUseAnimationEvent(1011);
                TipsMgr.Instance.ShowSystemTips("那边的门开了");
                TipsMgr.Instance.ShowTaskTips("请前往浑天禅院内");
                item.itemDetails.itemUseType = ItemUseType.None;
            }

            if (parent != null)
            {
                if (item != null)
                    item.itemDetails.itemUseType = ItemUseType.旋转;
            }
        }
        else if (!isTaskCompleted)
        {
            view.ClearRay();
            hasShowTips = false; // 镜子关掉时，重置提示
        }
    }

    private bool CheckReflectionOrderCorrect()
    {
        if (currentHitOrder.Count != reflectObjects.Count)
            return false;

        for (int i = 0; i < currentHitOrder.Count; i++)
        {
            if (currentHitOrder[i] != reflectObjects[i])
                return false;
        }
        return true;
    }

    private IEnumerator FadeLightRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        view.FadeOutLine(fadeDuration);
    }

    public void ResetTask()
    {
        isTaskCompleted = false;
        hasShowTips = false; // 重置任务时也重置
        currentHitOrder.Clear();
        view.ClearRay();
    }
}