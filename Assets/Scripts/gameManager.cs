using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("--- Player ---")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;
    public Animator playerAnim;
    public AudioSource playerAudio;

    [Header("---Game Goal---")]
    public int enemiesRemaining;
    [SerializeField] TextMeshProUGUI enemiesRemainingText;
    waveSpawner spawner;

    [Header("------UI------")]
    [Header("---Menus---")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    [Header("---Player UI---")]
    public Image playerHPBar;
    public GameObject screenFlash;  // ScreenFlash On TakeDamage
    [Header("---Active Weapon UI---")]
    public GameObject activeUI;
    public Image activeWeaponIcon;
    public TextMeshProUGUI activeCurrentAmmo;
    public TextMeshProUGUI activeMaxAmmo;
    public GameObject weaponReticle;
    public Sprite reticleBorder;
    [Header("---Inactive Weapon UI---")]
    public GameObject inactiveUI;
    public Image inactiveWeaponIcon;
    public TextMeshProUGUI inactiveCurrentAmmo;
    public TextMeshProUGUI inactiveMaxAmmo;
    
    // For Pauseing Game
    public bool isPaused = false;
    float timeScaleOrig;


    void Awake()
    {
        instance = this;
        
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerAnim = player.GetComponent<Animator>();
        playerAudio = player.GetComponent<AudioSource>();

        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");

        GameObject tspawn = GameObject.FindGameObjectWithTag("Spawner");
        if (GameObject.FindGameObjectsWithTag("Spawner") != null)
        {
            spawner = tspawn.GetComponent<waveSpawner>();
        }

        timeScaleOrig = Time.timeScale;

        // Set UI Inactive if weapons null
        if (playerScript.gunList.Count == 1)
        {
            inactiveUI.SetActive(false);
        }
        if (playerScript.gunList.Count == 0)
        {
            activeUI.SetActive(false);
            inactiveUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
                Pause();
            else
                UnPause();
        }
    }

    // Pause Game - Bring up Pause menu UI
    public void Pause()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        // Check to see if game is over based on enemy count <= 0
        if (enemiesRemaining <= 0 && spawner.WavesStopped() == true)
        {
            Pause();
            activeMenu = winMenu;
            activeMenu.SetActive(true);
        }
    }

    public void PlayerDead()
    {
        Pause();
        activeMenu = playerDeadMenu;
        activeMenu.SetActive(true);
    }

    public IEnumerator flashDamage()
    {
        screenFlash.SetActive(true);
        yield return new WaitForSeconds(.15f);
        screenFlash.SetActive(false);
    }

    public void UpdateUI()
    {
        GunStats active;
        GunStats inactive;

        if (playerScript.selectedGun == 0)
        {
            active = playerScript.gunList[0];
            UpdateActiveUI(active);
            if (playerScript.gunList.Count > 1)
            {
                inactive = playerScript.gunList[1];
                UpdateInactiveUI(inactive);
            }
        }
        else
        {
            active = playerScript.gunList[1];
            UpdateActiveUI(active);
            inactive = playerScript.gunList[0];
            UpdateInactiveUI(inactive);
        }
    }

    private void UpdateActiveUI(GunStats active)
    {
        // Set Active UI
        gameManager.instance.activeUI.SetActive(true);
        activeWeaponIcon.sprite = active.iconUI;
        if (active.reticle != null)
            gameManager.instance.weaponReticle.SetActive(true);
        else
            gameManager.instance.weaponReticle.SetActive(false);
            
        gameManager.instance.reticleBorder = active.reticle;

        activeCurrentAmmo.text = active.currentAmmo.ToString();
        activeMaxAmmo.text = active.currentMaxAmmo.ToString();
    }

    private void UpdateInactiveUI(GunStats inactive)
    {
        // Set Inactive UI
        gameManager.instance.inactiveUI.SetActive(true);
        inactiveWeaponIcon.sprite = inactive.iconUI;
        inactiveCurrentAmmo.text = inactive.currentAmmo.ToString();
        inactiveMaxAmmo.text = inactive.currentMaxAmmo.ToString();
    }

    public void UpdateActiveAmmo()
    {
        activeCurrentAmmo.text = playerScript.gunList[playerScript.selectedGun].currentAmmo.ToString();
        activeMaxAmmo.text = playerScript.gunList[playerScript.selectedGun].currentMaxAmmo.ToString();
    }

    public void UpdateInactiveAmmo()
    {
        if (playerScript.gunList[playerScript.selectedGun] == playerScript.gunList[0] && playerScript.gunList.Count == 2)
        {
            inactiveCurrentAmmo.text = playerScript.gunList[1].currentAmmo.ToString();
            inactiveMaxAmmo.text = playerScript.gunList[1].currentMaxAmmo.ToString();
        }
        else
        {
            inactiveCurrentAmmo.text = playerScript.gunList[0].currentAmmo.ToString();
            inactiveMaxAmmo.text = playerScript.gunList[0].currentMaxAmmo.ToString();
        }
    }
}