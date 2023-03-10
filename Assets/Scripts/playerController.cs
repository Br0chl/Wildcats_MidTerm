using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("---Components---")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("---Player Stats---")]
    [SerializeField] public int totalCurrency;
    public int currentHP;
    [Range(1, 300)] [SerializeField] public int maxHP;
    [Range(1, 10)] [SerializeField] public int walkSpeed;
    [Range(1,20)][SerializeField] int sprintSpeed;
    [Range(5, 20)] [SerializeField] int jumpAmount;
    [Range(5, 50)] [SerializeField] int gravity;
    [Range(1, 5)] [SerializeField] int jumpsAllowed;
    int overrideWalkSpeed;

    [Header("---Gun Stats---")]
    [SerializeField] public List<GunStats> gunList = new List<GunStats>();
    public int selectedGun;         // int to track selectedGun
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject bullethole;
    public GameObject shootPos;
    public ParticleSystem flameThrowerPart;
    public ParticleSystem flameThrowerOutofAmmo;
    public ParticleSystem handgunMuzzleFlash;
    public ParticleSystem smgMuzzleFlash;
    public ParticleSystem arMuzzleFlash;
    public ParticleSystem shotgunMuzzleFlash;
    public ParticleSystem lmgMuzzleFlash;

    [Header("---Equipment---")]
    [SerializeField] public Throwable equipment;
    [SerializeField] GameObject throwPos;
    public GameObject grenadeScreenItem;
    public GameObject knifeScreenItem;

    [Header("---Audio---")]
    [SerializeField] AudioClip[] audPlayerDamage;
    [Range(0, 1)][SerializeField] float audPlayerDamageVol;
    [SerializeField] AudioClip[] audPlayerJump;
    [Range(0, 1)][SerializeField] float audPlayerJumpVol;
    [SerializeField] AudioClip[] audPlayerSteps;
    [Range(0, 1)][SerializeField] float audPlayerStepsVol;

    [Header("---Test View---")]
    [SerializeField] private float playerSpeed;

    int playerSpeedOriginal;

    int jumpedTimes; // Tracks jumps ex. Double Jump

    Vector3 move;
    Vector3 playerVelocity;

    bool isShooting;
    bool isPlayingSteps;
    bool isSprinting;
    public bool isReloading;
    public bool gameStarted;
    bool isSwapping;
    bool readyToThrow = true;

    bool timerActive;
    int timersInUse;
    bool dotActive;
    int dotDamage;
    float dotTimer;
    float dotTick;

    // Pushback effect
    public Vector3 pushBack;
    [SerializeField] int pushBackTime;

    private void Start() 
    {
        // Cache playerSpeed
        playerSpeedOriginal = walkSpeed;
        // Set and update player HP for UI purposes
        currentHP = maxHP;
        UpdatePlayerHP();
        RespawnPlayer();

        // if player starts with a weapon
        if (gunList.Count > 0)
        {
            // 1 Gun at start
            selectedGun = 0;
            InitialzeGun(gunList[0]);
            gameManager.instance.UpdateActiveUI(gunList[0]);
            gameManager.instance.UpdateActiveAmmo();

            // 2 Guns at start
            if (gunList.Count == 2)
                gameManager.instance.UpdateInactiveAmmo();
        }
    }

    void Update()
    {
        if (!gameManager.instance.isPaused && gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.G) && readyToThrow && gameManager.instance.playerScript.equipment != null)
            {
                if (equipment.type == ThrowType.Grenade)
                    grenadeScreenItem.SetActive(true);
                else if (equipment.type == ThrowType.Knife)
                    knifeScreenItem.SetActive(true);
                //if (equipment.type == ThrowType.Grenade)
                // if (Input.GetKeyUp(KeyCode.G))
                // {
                //     Throw();
                //     grenadeScreenItem.SetActive(false);
                // }
                // Throw();
                // grenadeScreenItem.SetActive(false);
                
            }

            if (Input.GetKeyUp(KeyCode.G))
            {
                Throw();
                grenadeScreenItem.SetActive(false);
                knifeScreenItem.SetActive(false);
            }

            // Check if Gun has any ammo
            if (gunList.Count > 0)
            {
                if (gunList[selectedGun].currentAmmo == 0 && gunList[selectedGun].currentMaxAmmo == 0)
                {
                    gunList[selectedGun].isOutOfAmmo = true;

                    if (gunList[selectedGun].type == WeaponType.Flamethrower)
                        flameThrowerPart.gameObject.SetActive(false);
                }
            }

            pushBack = Vector3.Slerp(pushBack, Vector3.zero, Time.deltaTime * pushBackTime);

            if (move.normalized.magnitude > .3f && !isPlayingSteps)
                StartCoroutine(playSteps());

            Movement();
            Sprint();
            SelectGun();

            if (gunList.Count > 0)
            {
                // Reload
                if (Input.GetKeyDown(KeyCode.R) && gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].type != WeaponType.GrenadeLauncher)
                {
                    if (isReloading || isSwapping)
                        return;

                    StartCoroutine(Reload());
                }
                if (gunList[selectedGun].type == WeaponType.Flamethrower && !isReloading && !isSwapping)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (!gunList[selectedGun].isOutOfAmmo)
                            flameThrowerPart.gameObject.SetActive(true);
                        //isShooting = true;
                        if (!isShooting)
                            StartCoroutine(Shoot());
                        // else if (gunList[selectedGun].isOutOfAmmo)
                        // {
                        //     flameThrowerPart.gameObject.SetActive(false);
                        // }
                    }
                    else
                    {
                        flameThrowerPart.gameObject.SetActive(false);
                        isShooting = false;
                    }
                }
                else
                {
                    // Shoot
                    if (!isShooting && Input.GetButton("Shoot") && !isReloading && !isSwapping && gunList[selectedGun].type != WeaponType.Flamethrower)
                    {
                        StartCoroutine(Shoot());
                    }
                }
            }
            if (timerActive)
            { HandleTimers(); }
        }
    }

    void Movement()
    {
        if (controller.isGrounded && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal") +
               (transform.forward * Input.GetAxis("Vertical")));

        if (gameManager.instance.isSpeedUp)
            controller.Move(move * Time.deltaTime * playerSpeed * 2);
        else
            controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsAllowed)
        {
            playerVelocity.y = jumpAmount;
            jumpedTimes++;
            aud.PlayOneShot(audPlayerJump[Random.Range(0, audPlayerJump.Length)], audPlayerJumpVol);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move((playerVelocity + pushBack) * Time.deltaTime);
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed *= sprintSpeed;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed /= sprintSpeed;
        }
    }

    public void OverrideSpeed(int speed, int duration)
    {
        HandleSpeedOverride(speed, duration);
    }

    IEnumerator HandleSpeedOverride(int speed, int duration)
    {
        if (isSprinting)
        {
            playerSpeed /= sprintSpeed;
            playerSpeed = speed;
            playerSpeed *= sprintSpeed;
        }
        else
        { playerSpeed = speed; }

        yield return new WaitForSeconds(duration);

        if (isSprinting)
        {
            playerSpeed /= sprintSpeed;
            playerSpeed = playerSpeedOriginal;
            playerSpeed *= sprintSpeed;
        }
        else
        { playerSpeed = playerSpeedOriginal; }
    }

    public void AssignPushback(Vector3 toAdd)
    {
        pushBack = toAdd;
    }

    IEnumerator playSteps()
    {
        if (controller.isGrounded)
        {
            isPlayingSteps = true;
            aud.PlayOneShot(audPlayerSteps[Random.Range(0, audPlayerSteps.Length)], audPlayerStepsVol);

            if (!isSprinting)
                yield return new WaitForSeconds(0.5f);
            else
                yield return new WaitForSeconds(0.3f);

            isPlayingSteps = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        // Play sound based on ammo
        if (gunList[selectedGun].isOutOfAmmo)
        {
            if (gunList[selectedGun].type == WeaponType.Flamethrower)
            {
                aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
                flameThrowerOutofAmmo.Play();
                yield return new WaitForSeconds(2);
            }
            else
            {
                aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            }
        }
        // else if (gunList[selectedGun].type != WeaponType.Flamethrower)
        //     //aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);

        // if (gunList[selectedGun].type != WeaponType.Flamethrower && !gunList[selectedGun].isOutOfAmmo)
        //     gameManager.instance.playerAnim.SetTrigger("Shoot");

        // // Update current ammo or reload if needed
        // if (gunList[selectedGun].currentAmmo == 0 && gunList[selectedGun].currentMaxAmmo != 0)
        // {
        //     gunList[selectedGun].currentAmmo--;
            
        //     StartCoroutine(Reload());
            
        // }
        // else if (!gunList[selectedGun].isOutOfAmmo)
        // {
        //     gunList[selectedGun].currentAmmo--;
        // }

        // gameManager.instance.UpdateActiveAmmo();

        RaycastHit hit;
        if (!gunList[selectedGun].isOutOfAmmo)
        {

            // Shotgun Fire
            if (gunList[selectedGun].type == WeaponType.Shotgun)
            {
                for (int i = 0; i < gunList[selectedGun].shots; i++)
                {
                    Vector3 direction = Camera.main.transform.forward;
                    Vector3 spread = Vector3.zero;
                    spread += Camera.main.transform.up * Random.Range(-.1f, .1f);
                    spread += Camera.main.transform.right * Random.Range(-.1f, .1f);
                    direction += spread.normalized * Random.Range(0f, gunList[selectedGun].shotAngle);
                    if (Physics.Raycast(Camera.main.transform.position, direction, out hit, shootDist))
                    {
                        if (hit.collider.GetComponent<isDamageable>() != null)
                        {
                            hit.collider.GetComponent<isDamageable>().TakeDamage(shootDamage);
                        }
                        else
                        {
                            if (!gunList[selectedGun].isOutOfAmmo && hit.transform.tag != "Enemy" && hit.transform.tag != "Player")
                                Instantiate(bullethole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        }
                    }
                }
            }
            if (gunList[selectedGun].type == WeaponType.GrenadeLauncher)
            {
                gameManager.instance.playerAnim.SetTrigger("Shoot");
                //if (!gunList[selectedGun].isOutOfAmmo)
                    Instantiate(gunList[selectedGun].ammoToInstantiate, shootPos.transform);
            }
            if (gunList[selectedGun].type == WeaponType.Flamethrower)
            {
                if (gunList[selectedGun].currentAmmo == 1 && gunList[selectedGun].currentMagazines == 0)
                {
                    gunList[selectedGun].currentAmmo = 0;
                    flameThrowerPart.gameObject.SetActive(false);
                    gunList[selectedGun].isOutOfAmmo = true;
                }
                gameManager.instance.UpdateActiveAmmo();
            }
            else
            {
                // Bullet Spread
                Quaternion shotRotation = Quaternion.LookRotation(Camera.main.transform.forward);
                Quaternion randRotation = Random.rotation;
                float currentSpread = Mathf.Lerp(gunList[selectedGun].maxBulletSpread, 0.0f ,shootRate / gunList[selectedGun].timeToMaxSpread);
                shotRotation = Quaternion.RotateTowards(shotRotation, randRotation, Random.Range(0.0f, currentSpread));
                // if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                if (Physics.Raycast(Camera.main.transform.position, shotRotation * Vector3.forward, out hit, shootDist))
                {
                    if (hit.collider.GetComponent<isDamageable>() != null)
                    {
                        hit.collider.GetComponent<isDamageable>().TakeDamage(shootDamage);
                    }
                    else //if (hit.collider.CompareTag("Untagged"))
                    {
                        if (!gunList[selectedGun].isOutOfAmmo && hit.transform.tag != "Enemy" && hit.transform.tag != "Player" && hit.transform.tag != "Untagged")
                            Instantiate(bullethole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    }
                }
            }
        }

        // Update current ammo or reload if needed
        if (!gunList[selectedGun].isOutOfAmmo)
        {
            gunList[selectedGun].currentAmmo--;

            if (gunList[selectedGun].type != WeaponType.Flamethrower && !gunList[selectedGun].isOutOfAmmo && gunList[selectedGun].type != WeaponType.GrenadeLauncher)
                gameManager.instance.playerAnim.SetTrigger("Shoot");

            // gameManager.instance.UpdateActiveAmmo();
            // if (gunList[selectedGun].currentAmmo == 0 && gunList[selectedGun].currentMaxAmmo != 0)
            // {
            //     StartCoroutine(Reload());
            //     isShooting = false;
            //     yield break;
            // }
        }

        yield return new WaitForSeconds(shootRate);

        if (gunList[selectedGun].currentAmmo == 0 && gunList[selectedGun].currentMaxAmmo != 0)
        {
            StartCoroutine(Reload());
            isShooting = false;
            //yield break;
        }

        isShooting = false;
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        if (gunList[selectedGun].type == WeaponType.Flamethrower)
        {
            flameThrowerPart.gameObject.SetActive(false);
        }

        if (gunList[selectedGun].currentAmmo == gunList[selectedGun].magCapacity)
        {
            aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            isReloading = false;
            yield break;
        }
        else if (gunList[selectedGun].currentAmmo > 0 && gunList[selectedGun].currentMaxAmmo == 0)
        {
            aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            isReloading = false;
        }
        else if (gunList[selectedGun].isOutOfAmmo)
        {
            aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            isReloading = false;
            yield break;
        }
        else
        {
            gameManager.instance.playerAnim.SetTrigger("Reload");
            //aud.PlayOneShot(gunList[selectedGun].gunReloadAud, gunList[selectedGun].gunReloadAudVol);

            //yield return new WaitForSeconds(gunList[selectedGun].reloadSpeed);
            // Decrease Current Max ammo by mag capacity-currentAmmo and update UI
            if (gunList[selectedGun].currentMaxAmmo >= gunList[selectedGun].magCapacity)
            {
                gunList[selectedGun].currentMaxAmmo -= gunList[selectedGun].magCapacity - gunList[selectedGun].currentAmmo;
               // gameManager.instance.activeMaxAmmo.text = gunList[selectedGun].currentMaxAmmo.ToString();
                gunList[selectedGun].currentAmmo = gunList[selectedGun].magCapacity;
                //gameManager.instance.activeCurrentAmmo.text = gunList[selectedGun].magCapacity.ToString();
                if (gunList[selectedGun].currentMagazines > 0)
                    gunList[selectedGun].currentMagazines--;
            }
            else
            {
                int temp = gunList[selectedGun].magCapacity - gunList[selectedGun].currentAmmo;
                gunList[selectedGun].currentAmmo += temp;
                //gameManager.instance.activeCurrentAmmo.text = gunList[selectedGun].currentAmmo.ToString();
                if (gunList[selectedGun].currentMaxAmmo - temp < 0)
                    gunList[selectedGun].currentMaxAmmo = 0;
                else
                    gunList[selectedGun].currentMaxAmmo = gunList[selectedGun].currentMaxAmmo - temp;
                //gameManager.instance.activeMaxAmmo.text = gunList[selectedGun].currentMaxAmmo.ToString();
                if (gunList[selectedGun].currentMagazines > 0)
                    gunList[selectedGun].currentMagazines--;
            }
        }
        if (gunList[selectedGun].currentMagazines == gunList[selectedGun].maxMagazines)
            gunList[selectedGun].isAmmoFull = true;
        else
            gunList[selectedGun].isAmmoFull = false;

        yield return new WaitForSeconds(gunList[selectedGun].reloadSpeed);

        isReloading = false;
    }


    public void takeDamage(int dmg)
    {
        // If invincible powerup is active
        if (gameManager.instance.isInvinvcible)
            dmg = 0;
        currentHP -= dmg;
        UpdatePlayerHP();
        StartCoroutine(gameManager.instance.flashDamage());
        aud.PlayOneShot(audPlayerDamage[Random.Range(0, audPlayerDamage.Length)], audPlayerDamageVol);

        if (currentHP <= 0)
        {
            gameManager.instance.PlayerDead();
        }
    }

    public void TakeDamageOverTime(int damage, float time, float tick)
    {
        if (!timerActive)
        {
            timerActive = true;
            timersInUse++;
        }
        else
        { timersInUse++; }

        dotDamage = damage;
        dotTimer = time;
        dotTick = tick;
    }

    public void Heal(int healtToRestore)
    {
        if (maxHP - currentHP > healtToRestore)
            currentHP += healtToRestore;
        else
            currentHP = maxHP;
        UpdatePlayerHP();
    }

    public void UpdatePlayerHP()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    public void RespawnPlayer()
    {
        controller.enabled = false;
        currentHP = maxHP;
        UpdatePlayerHP();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    void HandleTimers()
    {
        if (dotTimer >= 0)
        {
            dotTimer -= Time.deltaTime;
            if (!dotActive)
            { StartCoroutine(DotTick()); }
        }
        else
        {
            dotTimer = -1;
            timersInUse--;
        }

        if (timersInUse <= 0)
        { timerActive = false; }
    }

    IEnumerator DotTick()
    {
        dotActive = true;
        yield return new WaitForSeconds(dotTick);
        takeDamage(dotDamage);
        dotActive = false;
    }

    public void gunPickup(GunStats gunStat)
    {
        // Unlock gun
        if (!gunStat.isUnlocked)
        {
            gunStat.isUnlocked = true;
            gunStat.gunLevel++;
        }

        foreach (GunStats gs in gunList)
        {
            if (gs == gunStat)
            {
                if (!gs.isAmmoFull)
                {
                    gs.currentMagazines += 1;
                    gs.currentMaxAmmo = gs.magCapacity * gs.currentMagazines;
                }

                if (gs.currentMagazines == gs.maxMagazines)
                {
                    if (gs.currentAmmo != 0)
                        if (gs == gunList[0] && gs == gunList[selectedGun])
                            StartCoroutine(gameManager.instance.MaxAmmoPopUp(0));
                        else if (gs == gunList[1] && gs == gunList[selectedGun])
                            StartCoroutine(gameManager.instance.MaxAmmoPopUp(0));
                        else
                            StartCoroutine(gameManager.instance.MaxAmmoPopUp(1));
                    gs.isAmmoFull = true;
                }
                else
                    gs.isAmmoFull = false;

                gameManager.instance.UpdateActiveAmmo();
                gameManager.instance.UpdateInactiveAmmo();
                if (gs.isOutOfAmmo && gs == gunList[selectedGun])
                {
                    gs.isOutOfAmmo = false;
                    StartCoroutine(Reload());
                }
                return;
            }
        }

        if (gunList.Count == 2)
            gunList.Remove(gunList[selectedGun]);

        gunList.Add(gunStat);
        InitialzeGun(gunStat);

        selectedGun = gunList.Count - 1;

        StartCoroutine(ChangeGun());

        gameManager.instance.UpdateUI();
    }

    public void InitialzeGun(GunStats gunStat)
    {
        shootRate = gunStat.shootRate;
        shootDist = gunStat.shootDist;
        shootDamage = gunStat.shootDamage;

        gunStat.currentAmmo = gunStat.magCapacity;
        gunStat.currentMagazines = gunStat.startingMagazines;
        gunStat.currentMaxAmmo = gunStat.magCapacity * gunStat.startingMagazines;

        gunStat.isOutOfAmmo = false;
        if (gunStat.currentMagazines == gunStat.maxMagazines)
            gunStat.isAmmoFull = true;
        else
            gunStat.isAmmoFull = false;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        gameManager.instance.playerAnim.runtimeAnimatorController = gunList[selectedGun].anim;
    }

    void SelectGun()
    {
        if (isReloading || isShooting || isSwapping || gunList.Count < 2)
            return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)// && selectedGun < gunList.Count - 1)
        {
            isSwapping = true;
            if (selectedGun == 1)
                selectedGun = 0;
            else
                selectedGun++;

            StartCoroutine(ChangeGun());
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)// && selectedGun > 0)
        {
            isSwapping = true;
            if (selectedGun == 0)
                selectedGun = 1;
            else
                selectedGun--;

            StartCoroutine(ChangeGun());
        }
    }

    IEnumerator ChangeGun()
    {
        StartCoroutine(SwapTime());
        gameManager.instance.playerAnim.SetTrigger("SwapOut");
        yield return new WaitForSeconds(.5F);
        shootRate = gunList[selectedGun].shootRate;
        shootDist = gunList[selectedGun].shootDist;
        shootDamage = gunList[selectedGun].shootDamage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gameManager.instance.playerAnim.runtimeAnimatorController = gunList[selectedGun].anim;

        gameManager.instance.playerAnim.SetTrigger("SwapIn");
        yield return new WaitForSeconds(0.5f);

        // Ammo Check
        if (gunList[selectedGun].isOutOfAmmo)
        {
            if (gunList[selectedGun].currentAmmo != 0 || gunList[selectedGun].currentMagazines != 0)
            {
                gunList[selectedGun].isOutOfAmmo = false;
                StartCoroutine(Reload());
            }
        }

        gameManager.instance.UpdateUI();
        isSwapping = false;
    }

    void Throw()
    {
        if (equipment == null) return;
        readyToThrow = false;

        // Instantiate Object to be thrown
        GameObject projectile = Instantiate(equipment.gameObject, throwPos.transform.position, Camera.main.transform.rotation);

        // Calculate the direction from throw position to aim position
        Vector3 forceDirection = Camera.main.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 25f))
        {
            forceDirection = (hit.point - throwPos.transform.position).normalized;
        }

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        // Add force
        Vector3 forceToAdd = forceDirection * equipment.throwForce + transform.up * equipment.throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        if (equipment.currentAmount > 1)
        {
            equipment.currentAmount--;
            gameManager.instance.UpdateEquipmentUI();
        }
        else
        {
            gameManager.instance.equipmentUI.SetActive(false);
            equipment = null;
            readyToThrow = true;
            return;
        }

        Invoke(nameof(ResetThrow), equipment.throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    IEnumerator SwapTime()
    {
       // isSwapping = true;
        yield return new WaitForSeconds(gunList[selectedGun].swapSpeed);
        //isSwapping = false;
    }

    public void AddCurrency(int amount)
    {
        // If currency powerup is active
        if (gameManager.instance.isDoubleCurrency)
            totalCurrency += amount * 2;
        else
            totalCurrency += amount;

        gameManager.instance.UpdateCurrencyUI();
    }

    public void UpdateShootDamage()
    {
        shootDamage = gunList[selectedGun].shootDamage;
    }

    public void ShootSound()
    {
        switch (gunList[selectedGun].type)
        {
            case WeaponType.Handgun:
                handgunMuzzleFlash.Play(true);
                break;
            case WeaponType.SMG:
                smgMuzzleFlash.Play(true);
                break;
            case WeaponType.AssaultRifle:
                arMuzzleFlash.Play(true);
                break;
            case WeaponType.Shotgun:
                shotgunMuzzleFlash.Play(true);
                break;
            case WeaponType.LMG:
                lmgMuzzleFlash.Play(true);
                break;
            default:
                break;
        }
        aud.clip = gunList[selectedGun].gunShotAud;
        aud.volume = gunList[selectedGun].gunShotAudVol;
        aud.Play();
        gameManager.instance.UpdateActiveAmmo();
    }

    public void ReloadSound()
    {
        aud.clip = gunList[selectedGun].gunReloadAud;
        aud.volume = gunList[selectedGun].gunReloadAudVol;
        aud.Play();
        gameManager.instance.UpdateActiveAmmo();
    }
}
