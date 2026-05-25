using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    public float rotationSpeed =10f; // 旋转速度
    private void OnTriggerStay(Collider other)
    {
        Item item = gameObject.GetComponent<Item>();

        if (item != null)
        {
            EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, true);

            if (Input.GetKey(KeyCode.E))
            {
                switch (item.itemDetails.itemUseType)
                {
                    case ItemUseType.安置:
                        // 检查子物体是否已经激活
                        bool hasActiveChildren = false;
                        foreach (Transform child in gameObject.transform)
                        {
                            if (child.gameObject.activeSelf)
                            {
                                hasActiveChildren = true;
                                break;
                            }
                        }
                        if (InventoryManager.Instance.CheckStock(item.itemDetails.itemId)&&!hasActiveChildren)
                        {
                            foreach (Transform child in gameObject.transform)
                            {
                                child.gameObject.SetActive(true);
                                EventHandler.CallBuildFurnitureEvent(item.itemDetails.itemId);
                                TipsMgr.Instance.ShowSystemTips("安置成功");
                            }
                        }
                        break;

                    case ItemUseType.旋转:
                        gameObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                        break;
                 

                }

            }



        }
    }
    private void OnTriggerExit(Collider other)
    {
        Item item = gameObject.GetComponent<Item>();
        EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, false);
    }

}



