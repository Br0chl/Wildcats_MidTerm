using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] int dropChance;
    public GameObject itemToInstantiate;

    private void Start()
    {
        itemToInstantiate = this.gameObject;
    }

    public int GetDropChance()
    {
        return dropChance;
    }

    public void SetDropChance(int dChance)
    {
        dropChance = dChance;
    }
}
