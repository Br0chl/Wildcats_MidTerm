using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour
{
    public LevelData level;

    [SerializeField] Image levelImage;
    [SerializeField] TextMeshProUGUI strName;
    [SerializeField] TextMeshProUGUI wavesCompleted;
    [SerializeField] GameObject lockedPanel;

    public void AddItem(LevelData newLevel)
    {
        level = newLevel;

        levelImage.sprite = newLevel.levelImage;
        levelImage.enabled = true;
        strName.text = newLevel.strName;
        wavesCompleted.text = newLevel.highestWaveCompleted.ToString();

        if (!newLevel.isUnlocked)
            lockedPanel.SetActive(true);
        else
            lockedPanel.SetActive(false);

        CheckUnlock(newLevel);
    }

    public void ClearSlot()
    {
        levelImage.enabled = false;
        strName.text = "";
        wavesCompleted.text = "";
    }

    void CheckUnlock(LevelData level)
    {
        if (level.highestWaveCompleted >= 10)
        {
            level.levelToUnlock.isUnlocked = true;
        }
    }
}
