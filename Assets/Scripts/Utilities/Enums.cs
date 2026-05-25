using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Interact, // 交互物品
    PickUp,//拾取物品
    Animation,//动画物品
    
}
public enum ItemUseType
{
    None,
    使用, 
    旋转, 
    安置, 
    拾取, 
    启动, 
    查看, 
    锻造,

}
public enum ItemAninmation
{
    None,
    Door,
    SiNan,
}
public enum JudgeType
{
    Miss,
    Normal,
    Good,
    Perfect,
}

// 新增：任务类型枚举（对应4个游戏）
public enum TaskType
{

    Duanzao, //锻造
    SuZi,  // 数字华容道
    Mirror,// 铜镜反射
    FoXiang,     // 对应解密
    Music,         // 音游
    SiXiang     // 对应解密
    
}
// 新增：任务状态枚举
public enum TaskStatus
{
    UnCompleted, // 未完成
    Completed,   // 已完成（未领奖）
}


