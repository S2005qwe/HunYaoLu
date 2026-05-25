using System.Collections.Generic;
using UnityEngine;

/**
 * Title: 管理所有的Tips
 * Description:
 */
public class TipsMgr : Singleton<TipsMgr>
{
    private List<GameObject> _itemTipsLst = new List<GameObject>();
    // 缓存 TaskTips 组件，避免每次 Find
    private TaskTips _taskTips;
    public void ShowSystemTips(string msg)
    {
        
        // 从 Resources 加载提示预制体
        GameObject go = Resources.Load<GameObject>("SystemTips");

        // 实例化并设置父物体、位置、缩放
        GameObject tips = Instantiate(go);
        tips.transform.SetParent(GameObject.Find("Canvas").transform);
        tips.transform.localPosition = new Vector2(0, 160);
        tips.transform.localScale = Vector3.one;

        // 加入列表管理
        _itemTipsLst.Add(tips);
        SystemTips systemTips = tips.GetComponent<SystemTips>();
        if (systemTips != null)
        {
            systemTips.RefreshUI(msg);
        }
    }

    private void Start()
    {
        ShowTaskTips("前往山门前按E键跟老丈对话");
    }
    public void ShowTaskTips(string msg)
    {
       
        TaskTips.Instance.ShowSystemTips(msg);
    }

    public void CloseItemTips()
    {
        if (_itemTipsLst.Count <= 0)
        {
            return;
        }

        // 修复遍历删除BUG，必须倒序
        for (int i = _itemTipsLst.Count - 1; i >= 0; i--)
        {
            Destroy(_itemTipsLst[i]);
            _itemTipsLst.RemoveAt(i);
        }
    }
}