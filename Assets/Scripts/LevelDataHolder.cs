using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataHolder : MonoBehaviour
{
    [SerializeField] LevelData level;

    private void Start() 
    {
        gameManager.instance.currentLevel = level;
    }
}
