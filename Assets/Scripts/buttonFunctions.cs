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
}
