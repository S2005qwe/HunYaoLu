using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI: MonoBehaviour,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private Text amountText;

    [SerializeField]
    private Button button;

    public bool isSelected;

    public int slotIndex;

    public ItemDetails itemDetails;

    public int itemAmount;


    public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    private void Start()
    {
        isSelected = false;
        if (itemDetails == null)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// 更新格子UI和信息
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">持有数据</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        Debug.Log("更新UI");
        itemDetails = item;
        slotImage.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        slotImage.enabled = true;
        button.interactable = true;
    }

    /// <summary>
    /// 将Slot更新为空
    /// </summary>
    public void UpdateEmptySlot()
    {
        itemDetails = null;
        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetails == null)
            return;

        isSelected = !isSelected;
    }

    //拖拽时
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemAmount != 0)
        {
            inventoryUI.dragItem.enabled = true;
            inventoryUI.dragItem.sprite = slotImage.sprite;
            inventoryUI.dragItem.SetNativeSize();

            // 设置固定尺寸，避免图片过大
            RectTransform dragRect = inventoryUI.dragItem.GetComponent<RectTransform>();
            dragRect.sizeDelta = new Vector2(200, 200); // 设置合适的尺寸
        }
    }

    //拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.transform.position = Input.mousePosition;
       // Debug.Log("!");
    }

    //拖拽后
    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.enabled = false;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
           
            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemUI>();
            if (targetSlot == null)
            {
                Debug.Log("1111111111111111111111");
                EventHandler.CallDropItemEvent(itemDetails.itemId, new Vector3(0,0,0), itemDetails);

                return;
            }
            int targetIndex = targetSlot.slotIndex;

            Debug.Log("交换UI");

            InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
        }
    }
}
