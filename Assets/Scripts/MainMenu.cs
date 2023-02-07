using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject creditsPanel;
    public GameObject levelSelectMenu;
    public GameObject mainMenuDisplay;

    private void Start() 
    {
        mainMenuDisplay.SetActive(true);
    }

    public void OpenOptionsScreen()
    {
        mainMenuDisplay.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowCredits()
    {
        mainMenuDisplay.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void ShowLevelSelect()
    {
        mainMenuDisplay.SetActive(false);
        levelSelectMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsPanel.SetActive(false);

        mainMenuDisplay.SetActive(true);
    }
}