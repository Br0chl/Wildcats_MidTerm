using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void ShowSelected()
    {
        foreach (LevelSlot level in slots)
        {
            if (level.isSelected)
                level.selectedPopUp.SetActive(true);
        }
    }

    public void ResetSelected()
    {
        foreach (LevelSlot level in slots)
        {
            level.isSelected = false;
            level.selectedPopUp.SetActive(false);
        }
    }

    public void LoadLevel()
    {
        foreach (LevelSlot level in slots)
        {
            if (level.isSelected)
            {
                SceneManager.LoadSceneAsync(level.level.buildIndex);
            }
        }
    }
}
