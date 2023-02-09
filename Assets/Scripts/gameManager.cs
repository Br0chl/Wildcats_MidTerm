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
    public int maxWaves;
    public int currWave;
    [SerializeField] public TextMeshProUGUI enemiesRemainingText;
    [SerializeField] public TextMeshProUGUI wavesRemainingText;
    public waveSpawner spawner;
    [Header("---Level Unlocking---")]
    // Used to track level unlocks
    [SerializeField] GameObject levelUnlockUI;
    public LevelData currentLevel;

    [Header("------UI------")]
    public GameObject hudUI;
    [Header("---Menus---")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    public GameObject difficultyMenu;
    [Header("---Player UI---")]
    public Image playerHPBar;
    [SerializeField] TextMeshProUGUI currencyTextUI;
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
    [Header("---Equipment UI---")]
    public GameObject equipmentUI;
    public Image equipmentIcon;
    public TextMeshProUGUI currentEquipmentAmmo;
    [Header("---Shop Currency UI---")]
    public TextMeshProUGUI currencyShop;
    public TextMeshProUGUI currencyWBench;
    [Header("---PowerUps---")]
    public bool isInvinvcible;
    public bool isDoubleCurrency;
    public bool isDamageUp;
    public bool isSpeedUp;
    
    // For Pauseing Game
    public bool isPaused = false;
    float timeScaleOrig;

    // Keep pause menu from activating in shop
    public bool isShopping = false;

    void Awake()
    {
        instance = this;
        
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerAnim = player.GetComponent<Animator>();
        playerAudio = player.GetComponent<AudioSource>();

        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");

        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<waveSpawner>();

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
       UpdateCurrencyUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null && !isShopping)
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

    public void updateWaves(int Wave)
    {
        currWave = Wave;

        // Update highestwaveCompleted for level unlocking
        if (currentLevel.highestWaveCompleted < currWave - 1)
            currentLevel.highestWaveCompleted = currWave - 1;
        // Show level unlock if not unlocked
        if (currentLevel.highestWaveCompleted == currentLevel.wavesToUnlock && !currentLevel.levelToUnlock.isUnlocked)
        {
            currentLevel.levelToUnlock.isUnlocked = true;
            StartCoroutine(ShowLevelUnlock());
        }

        wavesRemainingText.text = currWave.ToString("F0") + '/' + maxWaves.ToString("F0");

    }

    public void PlayerDead()
    {
        Pause();
        activeMenu = playerDeadMenu;
        activeMenu.SetActive(true);
    }

    public void PlayerWin()
    {
        Pause();
        activeMenu = winMenu;
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

        playerScript.UpdatePlayerHP();
        UpdateCurrencyUI();
    }

    public void UpdateActiveUI(GunStats active)
    {
        // Set Active UI
        activeUI.SetActive(true);
        activeWeaponIcon.sprite = active.iconUI;
        if (active.reticle != null)
            weaponReticle.SetActive(true);
        else
            weaponReticle.SetActive(false);
            
        reticleBorder = active.reticle;

        activeCurrentAmmo.text = active.currentAmmo.ToString();
        activeMaxAmmo.text = active.currentMaxAmmo.ToString();
    }

    public void UpdateInactiveUI(GunStats inactive)
    {
        // Set Inactive UI
        inactiveUI.SetActive(true);
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

    public void UpdateCurrencyUI()
    {
        currencyTextUI.text = playerScript.totalCurrency.ToString();
        if (isShopping)
        {
            currencyShop.text = playerScript.totalCurrency.ToString();
            currencyWBench.text = playerScript.totalCurrency.ToString();
        }
    }

    public void SelectDifficutly()
    {
        Pause();
        activeMenu = difficultyMenu;
        activeMenu.SetActive(true);
    }

    IEnumerator ShowLevelUnlock()
    {
        levelUnlockUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        levelUnlockUI.SetActive(false);
    }
}