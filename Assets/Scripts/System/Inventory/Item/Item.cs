
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    public int itemID;  //物品ID
    public ItemDetails itemDetails; //物品数据


    private void Start()
    {
        if (itemID != 0)
        {
            Init(itemID);
        }

    }
    //初始化
    public void Init(int ID)
    {
        itemID = ID;

        //Inventory获得当前数据
        itemDetails = InventoryManager.Instance.GetItemDetails(itemID);


        if (itemDetails.itemType == ItemType.Interact)
        {
            itemDetails.itemUseType = ItemUseType.安置;
            gameObject.AddComponent<ItemInteract>();
        }


    }
    
}


