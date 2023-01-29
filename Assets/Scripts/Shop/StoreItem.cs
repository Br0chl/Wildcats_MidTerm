using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoreItem : ScriptableObject
{
    [SerializeField] public string iName;
    [SerializeField] public int price;
    [SerializeField] public Sprite icon;
    [SerializeField] public GunStats gunStats;
    [SerializeField] public int hpToAdd;

    public bool isGun;
    public bool isUpgrade;
    public bool isHP;
}
