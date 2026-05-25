using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace SHYL.Inventory
{
    // 新增脚本，用于处理物品使用
    public class ItemManager : MonoBehaviour
    {
        private Dictionary<int, GameObject> itemCache = new Dictionary<int, GameObject>();
        private Transform PlayerTransform => FindObjectOfType<ItemPickUp>().transform;

        private Transform itemParent; //用于物品放在父物体下
        void OnEnable()
        {
            EventHandler.UseAnimationEvent += OnUseAnimationEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        }
        void OnDisable()
        {
            EventHandler.UseAnimationEvent -= OnUseAnimationEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }


        private void OnUseAnimationEvent(int ID)
        {
            GameObject itemObject = GetItemGameObject(ID);
            Debug.Log(itemObject);
            Debug.Log(ID);

            if (itemObject != null)
            {
                Animator animator = itemObject.GetComponent<Animator>();
                if (animator != null)
                {
                    // 设置open参数为true
                    animator.SetBool("Open", true);
                }
            }
            else
            {
                Debug.LogWarning($"Item with ID {ID} not found.");
            }



        }

        //加载场景后寻找父物体
        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
           
        }

        private void OnDropItemEvent(int ID, Vector3 mousepos, ItemDetails itemDetail)
        {
            if (itemDetail.buildPrefab == null) return;

            // 去掉父物体，直接生成
            Instantiate(itemDetail.buildPrefab,
                PlayerTransform.position - new Vector3(1.5f, 0, 0),
                Quaternion.identity);
        }
        public GameObject GetItemGameObject(int ID)
        {
            if (itemCache.ContainsKey(ID))
                return itemCache[ID];

            Item[] items = FindObjectsOfType<Item>();
            foreach (Item item in items)
            {
                if (item.itemID == ID)
                {
                    itemCache[ID] = item.gameObject;
                    return item.gameObject;
                }
            }
            return null;
        }

    }
}
