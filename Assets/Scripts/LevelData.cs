using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelName
{
    Fire,
    Water,
    Forest,
    Ice
}

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    [SerializeField] LevelName levelName;
    [SerializeField] public string strName;
    [SerializeField] int buildIndex;

    [SerializeField] public LevelData levelToUnlock;

    public Sprite levelImage;

    public int highestWaveCompleted;

    public bool isUnlocked;
}
