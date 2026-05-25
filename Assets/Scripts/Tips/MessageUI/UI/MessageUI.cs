using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class MessageUI : MonoBehaviour
{
    [SerializeField] public GameObject interactiveUI; // 拾取提示的 UI 元素
    [SerializeField] public Text interactiveText; // 拾取提示的 UI 元素
    [SerializeField] public GameObject noticUI; // 通知的 UI 元素
    [SerializeField] public Text noticText; // 通知的 UI 元素
    private RectTransform pickupPromptRect; // 拾取提示的 RectTransform
    private void Start()
    {
        // 获取拾取提示的 RectTransform
        if (interactiveUI != null)
            pickupPromptRect = interactiveUI.GetComponent<RectTransform>();
    }
    void OnEnable()
    {
        EventHandler.ShowInteractUI += OnShowMessageUI;
        EventHandler.ShowNoticUI += OnShowNoticUI;
    }
    void OnDisable()
    {
        EventHandler.ShowInteractUI -= OnShowMessageUI;
        EventHandler.ShowNoticUI -= OnShowNoticUI;
    }



    public void OnShowMessageUI(Transform pickupTransform, ItemUseType itemUseType, bool isShow)
    {
        if (!isShow)
        {
            interactiveUI.gameObject.SetActive(false);
            return;
        }
        if (interactiveUI == null || pickupPromptRect == null)
        {
            return;
        }

        Collider collider = pickupTransform.GetComponent<Collider>();
        if (collider == null)
        {
            return;
        }
        // 获取可拾取物体的中心位置
        Vector3 worldPosition = collider.bounds.center;

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // 添加偏移量（例如 Y 轴偏移 50 像素）
        screenPosition.y += 50;

        // 将屏幕坐标设置为拾取提示的位置
        pickupPromptRect.position = screenPosition;

        // 显示拾取提示
        interactiveUI.gameObject.SetActive(true);
        interactiveText.text = itemUseType.ToString();

    }

    private void OnShowNoticUI(string message)
    {
        noticUI.gameObject.SetActive(true);
        noticText.text = message;
    }
}



