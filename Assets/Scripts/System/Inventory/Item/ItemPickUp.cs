using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private Transform playerTransform;
    public GameObject BellGameObject;
    private void Start()
    {
        playerTransform = transform;
    }

    private void OnTriggerStay(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(item.itemDetails.itemType == ItemType.PickUp)
                {
                    //拾取物品添加到背包
                    InventoryManager.Instance.AddItem(item, true);
                    TipsMgr.Instance.ShowSystemTips($"拾取了{item.itemDetails.itemName}");
                }
                if (item.itemDetails.itemUseType == ItemUseType.锻造)
                {
                    BellGameObject.SetActive(true);
                  
                }
                if (item.itemDetails.itemUseType == ItemUseType.查看)
                {
                    if(item.itemDetails.itemName == "线索1")
                    {
                        Destroy(item.gameObject);
                        EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, false);
                        Debug.Log("1111111sjb1");
                        AddPictrue.Instance.AddImagesFromSet1();
                        TipsMgr.Instance.ShowSystemTips($"新增线索，请在右上角查看");
                        TipsMgr.Instance.ShowTaskTips("请根据线索尽快完成任务");
                    }
                   if (item.itemDetails.itemName == "线索2")
                    {
                        Destroy(item.gameObject);
                        EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, false);
                        AddPictrue.Instance.AddImagesFromSet3();
                        TipsMgr.Instance.ShowSystemTips($"新增线索，请在右上角查看");
                        TipsMgr.Instance.ShowTaskTips("请根据线索尽快完成任务");
                    }
                }

                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Item item = other.GetComponent<Item>();
        EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, false);
    }
}




