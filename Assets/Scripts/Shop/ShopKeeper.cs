using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] GameObject activatePopUp;
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject shopCam;

    bool isActivated;
    bool playerInTrigger;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInTrigger)
        {
            isActivated = !isActivated;
            ActivateShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            activatePopUp.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            activatePopUp.SetActive(false);
        }
    }


    void ActivateShop()
    {
        if (isActivated)
        {
            gameManager.instance.isShopping = true;
            //gameManager.instance.playerScript.enabled = false;
            shopCam.SetActive(true);
            gameManager.instance.playerScript.enabled = false;
            gameManager.instance.currencyShop.text = gameManager.instance.playerScript.totalCurrency.ToString();
            gameManager.instance.curHPShop.text = gameManager.instance.playerScript.currentHP.ToString();
            gameManager.instance.maxHPShop.text = gameManager.instance.playerScript.maxHP.ToString();
            gameManager.instance.hudUI.SetActive(false);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            activatePopUp.SetActive(false);
            shopUI.SetActive(true);
        }
        else if (!isActivated)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            shopCam.SetActive(false);
            gameManager.instance.playerScript.enabled = true;
            //gameManager.instance.playerScript.enabled = true;
            gameManager.instance.hudUI.SetActive(true);
            shopUI.SetActive(false);
            activatePopUp.SetActive(true);
            gameManager.instance.isShopping = false;
        }
    }
}
