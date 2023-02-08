using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.UnPause();
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }

    public void Restart()
    {
        gameManager.instance.UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayerRespawn()
    {
        gameManager.instance.playerScript.RespawnPlayer();
        gameManager.instance.UnPause();
    }

    public void ReturnToMain()
    {
        gameManager.instance.UnPause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadSceneAsync(0);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void EasyMode()
    {
        gameManager.instance.spawner.StartSpawner(5, 12);
    }

    public void MediumMode()
    {
        gameManager.instance.spawner.StartSpawner(7, 18);
    }

    public void HardMode()
    {
        gameManager.instance.spawner.StartSpawner(10, 24);
    }
}

