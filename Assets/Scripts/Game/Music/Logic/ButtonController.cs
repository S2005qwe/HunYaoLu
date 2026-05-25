using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : Singleton<ButtonController>
{
    public List<ButtonInfo> buttonInfos = new List<ButtonInfo>();
   
    public GameObject[] pressEffects; // 特效预制体
    [Range(0f, 1f)]
    public float pressedAlpha = 0.5f; // 按下时的透明度（0完全透明，1完全不透明）
    public float normalAlpha = 1f; // 正常状态透明度

    private Dictionary<Image, Color> originalColors = new Dictionary<Image, Color>(); // 存储每个按钮的原始颜色

    void Start()
    {
        // 初始化所有按钮为默认图片
        foreach (var info in buttonInfos)
        {
            if (info != null && info.theImage != null )
            {
             

                // 保存每个按钮的原始颜色
                if (!originalColors.ContainsKey(info.theImage))
                {
                    originalColors[info.theImage] = info.theImage.color;
                }

                // 确保初始透明度为正常值
                Color color = info.theImage.color;
                color.a = normalAlpha;
                info.theImage.color = color;
            }
        }
    }

    void Update()
    {
        foreach (var info in buttonInfos)
        {
            // 空值跳过
            if (info == null || info.theImage == null)
                continue;

            // 按键按下
            if (Input.GetKeyDown(info.keyToPress))
            {
                // 改变透明度为按下状态
                Color color = info.theImage.color;
                color.a = pressedAlpha;
                info.theImage.color = color;

                // 生成按键特效
                SpawnPressEffect(info);

                // 查找当前可按下的音符并触发判定
                FindAndPressNote();
            }

            // 按键抬起
            if (Input.GetKeyUp(info.keyToPress))
            {
                // 恢复透明度为正常状态
                Color color = info.theImage.color;
                color.a = normalAlpha;
                info.theImage.color = color;
            }
        }
    }

    /// <summary>
    /// 查找并触发可按下的音符
    /// </summary>
    void FindAndPressNote()
    {
        // 找到场景中所有可被按下的音符
        NoteObject[] notes = FindObjectsOfType<NoteObject>();
        foreach (NoteObject note in notes)
        {
            // 检查音符是否可以被按下并且处于激活状态
            if (note.canBePressed && note.gameObject.activeSelf)
            {
                note.Press();
                break; // 只触发一个音符
            }
        }
    }

    /// <summary>
    /// 生成按键特效的独立方法（作为按钮的子物体生成）
    /// </summary>
    void SpawnPressEffect(ButtonInfo info)
    {
        if (pressEffects == null || pressEffects.Length == 0)
            return;

        // 获取按钮图片的RectTransform
        RectTransform buttonRect = info.theImage.rectTransform;

        // 获取按钮中心坐标（世界坐标）
        Vector3 buttonCenter = buttonRect.position;

        foreach (GameObject effectPrefab in pressEffects)
        {
            if (effectPrefab == null)
                continue;

            // 实例化特效
            GameObject effect = Instantiate(effectPrefab, buttonCenter, Quaternion.identity);

            // 设置特效的父物体为按钮图片
            effect.transform.SetParent(buttonRect, false);

            // 如果是UI特效，确保位置本地化
            if (effect.GetComponent<RectTransform>() != null)
            {
                RectTransform effectRect = effect.GetComponent<RectTransform>();
                // 设置特效的锚点为中心
                effectRect.anchorMin = new Vector2(0.5f, 0.5f);
                effectRect.anchorMax = new Vector2(0.5f, 0.5f);
                effectRect.pivot = new Vector2(0.5f, 0.5f);
                // 设置本地位置为 (0, 0, 0)，使其位于按钮中心
                effectRect.localPosition = Vector3.zero;
            }

            // 自动销毁特效
            float destroyTime = 1f;
            if (effect.TryGetComponent(out ParticleSystem ps))
            {
                destroyTime = ps.main.duration;

                // 如果有ParticleSystem，确保自动播放并等待完成
                ps.Play();
            }

            Destroy(effect, destroyTime);
        }
    }
}