using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemDetails
{
    public int itemId;
    public string itemName;
    public GameObject buildPrefab;
    public ItemType itemType;                
    public Sprite itemIcon; //图标
    public ItemUseType itemUseType; //使用类型

}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}


[System.Serializable]
public class BluePrintDetails
{
    public int ID;

    public InventoryItem[] resourceItem = new InventoryItem[4];

}

// 新增：任务数据模型
[Serializable]
public class TaskDetails
{
    public int taskId;               // 任务ID
    public TaskType taskType;        // 任务类型（对应4个游戏）
    
    public TaskStatus taskStatus;    // 任务状态

}

[Serializable]
public class DropNoteInfo
{
    [Tooltip("音乐播放到该时间 音符开始下落")]
    public float fallStartTime;
}
//音符生成 
[System.Serializable]
public class NoteDat
{
    public int laneId; // 轨道编号 0、1、2、3...
    public List<DropNoteInfo> laneNotes; // 当前轨道所有下落音符
}

[System.Serializable]
public class SceneDetail
{
    [Header("场景名称（必须和Build Settings里一致）")]
    public string sceneName;

    [Header("是否是主场景（只能有一个）")]
    public bool isMainScene;

    [Header("加载时是否卸载其他场景")]
    public bool unloadOtherScenes = true;
}

[System.Serializable]
public class ButtonInfo
{
    public int TrackId;
    public Image theImage;          // 按钮的SpriteRenderer
    public KeyCode keyToPress;             // 触发按钮的按键

}

[System.Serializable]
public class GameOver
{
    public GameObject FireObject;
    public int Taskid;

}