using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    //******************************************
    [Header("DEFAULT SETTINGS")]
    [SerializeField] public int defaultDamage;
    [SerializeField] public int defaultMaxMags;
    [SerializeField] public int defaultStartMags;
    //******************************************
    [Header("---------------------")]
    
    [Header("---Base Stats---")]
    [SerializeField] public WeaponType type;
    public string iName;
    public int price;
    public int costToUpgrade;
    public float shootRate;
    public int shootDist;
    public int shootDamage;
    public float reloadSpeed;
    [Header("-----Shotgun Only-----")]
    public int shots;
    public float shotAngle;
    [Header("-----GrenadeLauncher Only-----")]
    public GameObject ammoToInstantiate;
    [Header("-----Bullet Spread Option-----")]
    public float maxBulletSpread;
    public float timeToMaxSpread;

    [Header("---Components---")]
    public GameObject gunModel;
    public AudioClip gunShotAud;
    [Range(0, 1)] public float gunShotAudVol;
    public AudioClip gunReloadAud;
    [Range(0, 1)] public float gunReloadAudVol;
    public AudioClip gunAmmoOutAud;
    [Range(0, 1)] public float gunAmmoOutAudVol;
    public AnimatorOverrideController anim;

    [Header("---Ammo Tracking")]
    public int currentAmmo;
    public int currentMaxAmmo;
    public int currentMagazines;
    public int magCapacity;
    public int maxMagazines;
    public int startingMagazines;
    public float swapSpeed = 1f;
    public bool isOutOfAmmo = false;
    public bool isAmmoFull = false;

    [Header("---UI---")]
    public Sprite iconUI;
    public Sprite reticle;

    [Header("---Upgrading---")]
    public int gunLevel;
    public int damageUpgradeLevel;
    public int maxAmmoUpgradeLevel;
    public int startingMagsUpgradeLevel;
    public bool isUnlocked;
}
