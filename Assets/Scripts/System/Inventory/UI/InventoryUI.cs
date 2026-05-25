using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("拖拽图片")]
    public Image dragItem;  // 拖拽图片
    [Header("玩家背包UI")]
    [SerializeField] private GameObject bagUI;
    [SerializeField] private Image pickupPrompt; // 拾取提示图片
    [SerializeField] private GameObject itemPrefab;
    private RectTransform pickupPromptRect; // 拾取提示的 RectTransform
    private bool bagOpened = false; // 背包打开状态
    [SerializeField] private ItemUI[] playerSlots;

    private void Start()
    {
        bagUI.SetActive(bagOpened);
        // 设置槽位索引
        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].slotIndex = i;
        }
    }
    private void Update()
    {
        // 检测 F 键是否按下
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenBagUI();
        }
    }
    private void OnEnable()
    {
        EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
    }
    private void OnDisable()
    {
        EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
    }

    // 显示拾取提示


    private void OnUpdateInventoryUI(List<InventoryItem> list)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (list[i].itemAmount > 0)
            {
                var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                playerSlots[i].UpdateSlot(item, list[i].itemAmount);
            }
            else
            {
                playerSlots[i].UpdateEmptySlot();
            }
        }
    }

    public void OpenBagUI()
    {
        bagOpened = !bagOpened;

        bagUI.SetActive(bagOpened);
    }

}