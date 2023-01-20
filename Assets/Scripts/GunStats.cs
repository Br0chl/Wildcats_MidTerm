using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public float shootRate;
    public int shootDist;
    public int shootDamage;
    public float reloadSpeed;

    public GameObject gunModel;
    public AudioClip gunShotAud;
    [Range(0, 1)] public float gunShotAudVol;
    public AudioClip gunReloadAud;
    [Range(0, 1)] public float gunReloadAudVol;
    public AudioClip gunAmmoOutAud;
    [Range(0, 1)] public float gunAmmoOutAudVol;

    public int currentAmmo;
    public int currentMaxAmmo;
    public int currentMagazines;

    public int magCapacity;
    public int maxMagazines;
    public int startingMagazines;

    public float swapSpeed = 1f;

    public Sprite iconUI;
    public Sprite reticle;
}
