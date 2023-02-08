using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    public Transform parentObject;

    [SerializeField] List<LevelData> levelList = new List<LevelData>();

    LevelSlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        slots = parentObject.GetComponentsInChildren<LevelSlot>();
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < levelList.Count)
            {
                slots[i].AddItem(levelList[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
