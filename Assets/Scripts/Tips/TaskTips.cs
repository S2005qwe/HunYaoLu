using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTips : Singleton<TaskTips> 
{
    public Text taskText;
    // Start is called before the first frame update
    /// <summary>
    /// 更新提示文本（只改内容，不消失）
    /// </summary>
    public void ShowSystemTips(string msg)
    {
        if (taskText == null) return;

        taskText.text = msg;
    }
}
