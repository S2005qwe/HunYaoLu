using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class TaskGameManager : Singleton<TaskGameManager>
{
    public Task_SO taskData;
    private TaskDetails taskDetails;

    private void OnEnable()
    {
        EventHandler.TaskStatusUpdateEvent += OnTaskStatusUpdate;

    }
    private void OnDisable()
    {
        EventHandler.TaskStatusUpdateEvent -= OnTaskStatusUpdate;
    }

    public TaskDetails GetTaskDetails(int ID)
    {
        return taskData.taskDetailsList.Find(t => t.taskId == ID);
    }

    public bool IsTaskCompleted(int taskId)
    {
        // 1. 先根据ID找任务
        TaskDetails task = GetTaskDetails(taskId);

        // 2. 判断：
        //    如果 找到任务 + 状态是 Completed → 返回 true
        //    其他所有情况 → 返回 false
        return task != null && task.taskStatus == TaskStatus.Completed;
    }

    private void OnTaskStatusUpdate(int taskId, TaskStatus status)
    {
        TaskDetails taskDetails = GetTaskDetails(taskId);
       
        if (taskDetails!=null)
        {
           taskDetails.taskId = taskId;
           taskDetails.taskStatus = status;

        }
    }
}
