using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Database Settings")]
    public ItemDataList_SO itemDataList; // 物品数据列表
    public InventoryBag_SO PlayerBag; // 玩家背包
    public BluePrintDataList_SO bluePrintData;

    void OnEnable()
    {
        EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        EventHandler.DropItemEvent += OnDropItemEvent;
    }

   
    void OnDisable()
    {
        EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
        EventHandler.DropItemEvent -= OnDropItemEvent;
    }

    private void Start()
    {
        //更新背包UI
        EventHandler.CallUpdateInventoryUI(PlayerBag.itemList);
    }


    private void OnDropItemEvent(int ID, Vector3 pos, ItemDetails itemDetails)
    {
        ReMoveItem(ID, 1);
    }
    private void OnBuildFurnitureEvent(int ID)
    {
        if (bluePrintData != null)
        {
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            if (bluePrint != null)
            {
                foreach (var item in bluePrint.resourceItem)
                {
                    ReMoveItem(item.itemID, item.itemAmount);
                }
            }
            else
            {
                Debug.LogError($"No blueprint found for ID: {ID}");
            }
        }
    }

    public ItemDetails GetItemDetails(int ID)
    {
        //Find找item用于i代名，通过i.itemId->ID，返回物品数据的列表中第i个
        return itemDataList.itemsDetailsList.Find(i => i.itemId == ID);
    }

    /// <summary>
    /// 添加物品到Player背包里
    /// </summary>
    /// <param name="item"></param>
    /// <param name="toDestory"></param>

    public void AddItem(Item item, bool toDestory)
    {
        Debug.Log("碰到了");
        //是否已经有该物品
        var index = GetItemIndexInBag(item.itemID);
        AddItemAtIndex(item.itemID, index, 1);
        Debug.Log(GetItemDetails(item.itemID).itemId + "Name: " + GetItemDetails(item.itemID).itemName);

        //如果可以销毁，则销毁物品
        if (toDestory)
        {
            Destroy(item.gameObject);
        }
        EventHandler.CallShowInteractUI(item.transform, item.itemDetails.itemUseType, false);
        EventHandler.CallUpdateInventoryUI(PlayerBag.itemList);
    }

    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    /// <returns></returns>
    private bool CheckBagCapacity()
    {
        for (int i = 0; i < PlayerBag.itemList.Count; i++)
        {
            if (PlayerBag.itemList[i].itemID == 0)
                return true;
        }
        return false;
    }


    /// <summary>
    /// 通过物品ID找到背包已有物品位置
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <returns>1则没有这个物品否则返回序号</returns>
    private int GetItemIndexInBag(int ID)
    {
        if (PlayerBag == null || PlayerBag.itemList == null)
        {
            Debug.LogError("PlayerBag or itemList is null!");
            return -1;
        }
        for (int i = 0; i < PlayerBag.itemList.Count; i++)
        {
            if (PlayerBag.itemList[i].itemID == ID)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 在指定背包序号位置添加物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="index">序号</param>
    /// <param name="amount">数据</param>
    private void AddItemAtIndex(int ID, int index, int amount)
    {
        //通过索引来看，如果背包中没有这个物品,同时有空位
        if (index == -1 && CheckBagCapacity())
        {
            //则创建一个物品
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            //创建物品后，查找空位
            for (int i = 0; i < PlayerBag.itemList.Count; i++)
            {
                if (PlayerBag.itemList[i].itemID == 0)
                {
                    PlayerBag.itemList[i] = item;
                    break;
                }
            }
            Debug.Log("添加物品到背包");
        }
        else  //背包里有这个东西
        {
            int currentAmount = PlayerBag.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
            PlayerBag.itemList[index] = item;
            Debug.Log("背包里已有物品，叠加数量");
        }
    }


    /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = PlayerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }


    // 获取物品数量
    private int GetItemCount(int itemId)
    {
        foreach (InventoryItem item in PlayerBag.itemList)
        {
            if (item.itemID == itemId)
            {
                return item.itemAmount;
            }
        }
        return 0;
    }

    // 检查背包中是否存在特定物品
    public bool HasItemInInventory(int itemId)
    {
        foreach (InventoryItem item in PlayerBag.itemList)
        {
            if (item.itemID == itemId && item.itemAmount > 0)
            {
                Debug.Log($"背包里有{itemId}");
                return true;
            }
        }
        return false;
    }

    // 新增方法，用于获取玩家背包
    public List<InventoryItem> GetPlayerInventory()
    {
        return PlayerBag.itemList;
    }

    // 使用物品，更新背包数量
    public void UseItem(int itemId)
    {
        for (int i = 0; i < PlayerBag.itemList.Count; i++)
        {
            InventoryItem item = PlayerBag.itemList[i];
            if (item.itemID == itemId)
            {
                if (item.itemAmount > 0)
                {
                    PlayerBag.itemList[i] = new InventoryItem
                    {
                        itemID = item.itemID,
                        itemAmount = item.itemAmount - 1
                    };

                    if (PlayerBag.itemList[i].itemAmount == 0)
                    {
                        PlayerBag.itemList.RemoveAt(i); // 如果数量为 0，从背包移除
                    }
                    return;
                }
            }
        }

        Debug.LogError($"背包中没有足够的{itemId}快去寻找");
    }

    /// <summary>
    /// Player背包范围内交换物品
    /// </summary>
    /// <param name="fromIndex">起始序号</param>
    /// <param name="toIndex">目标数据序号</param>
    public void SwapItem(int fromIndex, int toIndex)
    {
        InventoryItem currentItem = PlayerBag.itemList[fromIndex];
        InventoryItem targetItem = PlayerBag.itemList[toIndex];

        if (targetItem.itemID != 0)
        {
            PlayerBag.itemList[fromIndex] = targetItem;
            PlayerBag.itemList[toIndex] = currentItem;
            Debug.Log("1");
        }
        else
        {
            PlayerBag.itemList[toIndex] = currentItem;
            PlayerBag.itemList[fromIndex] = new InventoryItem();
        }
        EventHandler.CallUpdateInventoryUI(PlayerBag.itemList);
    }

    //移除
    public void ReMoveItem(int ID, int removeAmount)
    {
        var index = GetItemIndexInBag(ID);
        if (index >= 0 && index < PlayerBag.itemList.Count) // 添加索引范围检查
        {
            if (PlayerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = PlayerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                PlayerBag.itemList[index] = item;
            }
            else if (PlayerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                PlayerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(PlayerBag.itemList);
        }
    }
}