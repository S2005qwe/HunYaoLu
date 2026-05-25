using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SHYL.Dialogue;
using UnityEngine;

public static class EventHandler
{
    //加载场景之后
    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }
    //更新背包UI
    public static event Action<List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(list);
    }

    public static event Action<Transform,ItemUseType,bool> ShowInteractUI;
    public static void CallShowInteractUI(Transform pickupTransform, ItemUseType itemUseType,bool isShow)
    {
        ShowInteractUI?.Invoke(pickupTransform, itemUseType,isShow);
    }

     public static event Action<string> ShowNoticUI;
    public static void CallShowNoticUI( string message)
    {
        ShowNoticUI?.Invoke(message);
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);   
    }
    
    //建造
    public static event Action<int> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID)
    {
        BuildFurnitureEvent?.Invoke(ID);
    }

    public static event Action<int > UseAnimationEvent;
    public static void CallUseAnimationEvent(int ID)
    {
        UseAnimationEvent?.Invoke(ID);
    }

    //扔东西
    public static event Action<int, Vector3, ItemDetails> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemDetails itemDetial)
    {
        DropItemEvent?.Invoke(ID, pos, itemDetial);
    }
   
 
        

    // 2. 任务状态更新事件（传递任务ID+状态）
    public static event Action<int, TaskStatus> TaskStatusUpdateEvent;
    public static void CallTaskStatusUpdateEvent(int taskId, TaskStatus status)
    {
        TaskStatusUpdateEvent?.Invoke(taskId, status);

    }

    //传送场景
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }



    //卸载场景之前
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }


    //玩家移动到目标位置
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    #region 拼图系统事件
    public static event Action PuzzleStartedEvent;
    public static void CallPuzzleStartedEvent()
    {
        PuzzleStartedEvent?.Invoke();
    }

    public static event Action PuzzleRestartedEvent;
    public static void CallPuzzleRestartedEvent()
    {
        PuzzleRestartedEvent?.Invoke();
    }
    // 显示拼图UI（你项目风格）
    public static event Action ShowPuzzleUIEvent;
    public static void CallShowPuzzleUIEvent()
    {
        ShowPuzzleUIEvent?.Invoke();
    }

    public static event Action HidePuzzleUIEvent;
    public static void CallHidePuzzleUIEvent()
    {
        HidePuzzleUIEvent?.Invoke();
    }
    #endregion
}
