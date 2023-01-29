using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopInventory : MonoBehaviour
{
    public Transform parentObject;

    [SerializeField] List<StoreItem> itemList = new List<StoreItem>();

    ItemSlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        slots = parentObject.GetComponentsInChildren<ItemSlot>();
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)
            {
                slots[i].AddItem(itemList[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
