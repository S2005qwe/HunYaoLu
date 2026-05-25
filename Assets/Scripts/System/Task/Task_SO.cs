using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///物品数据
/// </summary>
[CreateAssetMenu(fileName = "Task_SO", menuName = "Task/taskDetailsList")]
public class Task_SO : ScriptableObject
{
    public List<TaskDetails> taskDetailsList;
}
