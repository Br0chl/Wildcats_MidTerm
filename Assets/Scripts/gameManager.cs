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

    [Header("---Game Goal---")]
    public int enemiesRemaining;

    [Header("---UI---")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    public Image playerHPBar;
    [SerializeField] TextMeshProUGUI enemiesRemainingText;

    // For Pauseing Game
    public bool isPaused = false;
    float timeScaleOrig;


    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");

        timeScaleOrig = Time.timeScale;
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
        if (enemiesRemaining <= 0)
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
}