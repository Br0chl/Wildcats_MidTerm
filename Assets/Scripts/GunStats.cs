using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    [Header("---Base Stats---")]
    public string iName;
    public int price;
    public int costToUpgrade;
    public float shootRate;
    public int shootDist;
    public int shootDamage;
    public float reloadSpeed;

    [Header("---Components---")]
    public GameObject gunModel;
    public AudioClip gunShotAud;
    [Range(0, 1)] public float gunShotAudVol;
    public AudioClip gunReloadAud;
    [Range(0, 1)] public float gunReloadAudVol;
    public AudioClip gunAmmoOutAud;
    [Range(0, 1)] public float gunAmmoOutAudVol;

    [Header("---Ammo Tracking")]
    public int currentAmmo;
    public int currentMaxAmmo;
    public int currentMagazines;
    public int magCapacity;
    public int maxMagazines;
    public int startingMagazines;
    public float swapSpeed = 1f;
    public bool isOutOfAmmo;

    [Header("---UI---")]
    public Sprite iconUI;
    public Sprite reticle;

    [Header("---Upgrading---")]
    public int gunLevel;
    public int damageUpgradeLevel;
    public int maxAmmoUpgradeLevel;
    public int startingMagsUpgradeLevel;
}
