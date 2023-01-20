using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("---Components---")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("---Player Stats---")]
    [Range(1, 300)] [SerializeField] int maxHP;
    [Range(1, 10)] [SerializeField] int walkSpeed;
    [Range(1,20)][SerializeField] int sprintSpeed;
    [Range(5, 20)] [SerializeField] int jumpAmount;
    [Range(5, 50)] [SerializeField] int gravity;
    [Range(1, 5)] [SerializeField] int jumpsAllowed;

    [Header("---Gun Stats---")]
    [SerializeField] public List<GunStats> gunList = new List<GunStats>(2);
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] GameObject gunModel;

    [Header("---Audio---")]
    [SerializeField] AudioClip[] audPlayerDamage;
    [Range(0, 1)][SerializeField] float audPlayerDamageVol;
    [SerializeField] AudioClip[] audPlayerJump;
    [Range(0, 1)][SerializeField] float audPlayerJumpVol;
    [SerializeField] AudioClip[] audPlayerSteps;
    [Range(0, 1)][SerializeField] float audPlayerStepsVol;

    [Header("---Test View---")]
    [SerializeField] private float playerSpeed;

    // Current HP for healthbar
    public int currentHP;

    int playerSpeedOriginal;

    // int to track selectedGun
    public int selectedGun;

    int jumpedTimes; // Tracks jumps ex. Double Jump

    Vector3 move;
    Vector3 playerVelocity;

    bool isShooting;
    bool isPlayingSteps;
    bool isSprinting;
    bool isReloading;
    bool isSwapping;
    bool isJumping;

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
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            //pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackTime);
            pushBack.x = Mathf.Lerp(pushBack.x, 0, Time.deltaTime * pushBackTime);
            pushBack.z = Mathf.Lerp(pushBack.z, 0, Time.deltaTime * pushBackTime);
            pushBack.y = Mathf.Lerp(pushBack.y, 0, Time.deltaTime * pushBackTime * 3);

            if (move.normalized.magnitude > .3f && !isPlayingSteps)
                StartCoroutine(playSteps());

            Movement();
            Sprint();
            SelectGun();

            if (gunList.Count > 0)
            {
                // Reload
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (isReloading || isSwapping)
                        return;

                    StartCoroutine(Reload());
                }

                // Shoot
                if (!isShooting && Input.GetButton("Shoot") && !isReloading && !isSwapping)
                {
                    Debug.Log("SHOOT");
                    StartCoroutine(Shoot());
                }
            }
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
        if (gunList[selectedGun].currentAmmo == 0 && gunList[selectedGun].currentMaxAmmo == 0)
        {
            aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            gameManager.instance.playerAnim.SetTrigger("Reload");
            yield break;
        }
        else if (gunList[selectedGun].currentAmmo == 0)
        {
            StartCoroutine(Reload());
            yield break;
        }
        isShooting = true;

        aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);

        gameManager.instance.playerAnim.SetTrigger("Shoot");

        gunList[selectedGun].currentAmmo--;
        gameManager.instance.UpdateActiveAmmo();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            if (hit.collider.GetComponent<isDamageable>() != null)
            {
                hit.collider.GetComponent<isDamageable>().TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public IEnumerator Reload()
    {
        if (gunList[selectedGun].currentAmmo == gunList[selectedGun].magCapacity)
        {
            Debug.Log("Ammo Full");
            yield break;
        }
        else if (gunList[selectedGun].currentMaxAmmo == 0 && gunList[selectedGun].currentMaxAmmo == 0)
        {
            Debug.Log("Out of Ammo");
            aud.PlayOneShot(gunList[selectedGun].gunAmmoOutAud, gunList[selectedGun].gunAmmoOutAudVol);
            yield break;
        }
        else
        {
            Debug.Log("Reloading");
            isReloading = true;

            gameManager.instance.playerAnim.SetTrigger("Reload");
            aud.PlayOneShot(gunList[selectedGun].gunReloadAud, gunList[selectedGun].gunReloadAudVol);

            yield return new WaitForSeconds(gunList[selectedGun].reloadSpeed);
            // Decrease Current Max ammo by mag capacity-currentAmmo and update UI
            if (gunList[selectedGun].currentMaxAmmo >= gunList[selectedGun].magCapacity)
            {
                gunList[selectedGun].currentMaxAmmo -= gunList[selectedGun].magCapacity - gunList[selectedGun].currentAmmo;
                gameManager.instance.activeMaxAmmo.text = gunList[selectedGun].currentMaxAmmo.ToString();
                gunList[selectedGun].currentAmmo = gunList[selectedGun].magCapacity;
                gameManager.instance.activeCurrentAmmo.text = gunList[selectedGun].magCapacity.ToString();
                if (gunList[selectedGun].currentMagazines > 0)
                    gunList[selectedGun].currentMagazines--;
            }
            else
            {
                gunList[selectedGun].currentAmmo += gunList[selectedGun].currentMaxAmmo;
                gameManager.instance.activeCurrentAmmo.text = gunList[selectedGun].currentAmmo.ToString();
                gunList[selectedGun].currentMaxAmmo = 0;
                gameManager.instance.activeMaxAmmo.text = gunList[selectedGun].currentMaxAmmo.ToString();
                if (gunList[selectedGun].currentMagazines > 0)
                    gunList[selectedGun].currentMagazines--;
            }
            isReloading = false;
        }
    }


    public void takeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdatePlayerHP();
        StartCoroutine(gameManager.instance.flashDamage());
        aud.PlayOneShot(audPlayerDamage[Random.Range(0, audPlayerDamage.Length)], audPlayerDamageVol);

        if (currentHP <= 0)
        {
            gameManager.instance.PlayerDead();
        }
    }

    public void Heal(int healtToRestore)
    {
        currentHP += healtToRestore;
        UpdatePlayerHP();
    }

    private void UpdatePlayerHP()
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

    public void gunPickup(GunStats gunStat)
    {
        // if (gunList.Count > 0)
        // {
        //     StartCoroutine(ChangeGun());
        // }
        // else
        //     gameManager.instance.playerAnim.SetTrigger("SwapIn");


        if (gunList.Count == 2)
            gunList.Remove(gunList[selectedGun]);

        gunList.Add(gunStat);

        shootRate = gunStat.shootRate;
        shootDist = gunStat.shootDist;
        shootDamage = gunStat.shootDamage;

        gunStat.currentAmmo = gunStat.magCapacity;
        gunStat.currentMaxAmmo = gunStat.magCapacity * gunStat.startingMagazines;
        gunStat.currentMagazines = gunStat.startingMagazines;

        selectedGun = gunList.Count - 1;

        StartCoroutine(ChangeGun());

        gameManager.instance.UpdateUI();
    }

    void SelectGun()
    {
        if (isReloading || isShooting)
            return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            StartCoroutine(ChangeGun());
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
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

        gameManager.instance.playerAnim.SetTrigger("SwapIn");

        gameManager.instance.UpdateUI();
    }

    IEnumerator SwapTime()
    {
        isSwapping = true;
        yield return new WaitForSeconds(gunList[selectedGun].swapSpeed);
        isSwapping = false;
    }
}
