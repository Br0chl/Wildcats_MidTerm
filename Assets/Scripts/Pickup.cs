using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] int healthToRestore;
    [SerializeField] int currencyAmount;
    [SerializeField] int magazineAmount;

    [SerializeField] Throwable throwable;
    //[SerializeField] GameObject equipPopUp = null;
    public bool popUpActive = false;

    [SerializeField] float respawnTime = 0f;

    [SerializeField] GameObject soundPrefab;

    private void Update()
    {
        if (popUpActive && Input.GetKeyDown(KeyCode.E))
        {
            gameManager.instance.playerScript.equipment = throwable;
            gameManager.instance.playerScript.equipment.currentAmount = 1;
            gameManager.instance.UpdateEquipmentUI();
            gameManager.instance.pickUpPopup.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUp(other.gameObject);
        }
    }

    private void PickUp(GameObject subject)
    {
        // Heal Player - Health Pickup
        if (healthToRestore > 0)
        {
            if (gameManager.instance.playerScript.currentHP == gameManager.instance.playerScript.maxHP)
                return;
            else
            {
                gameManager.instance.playerScript.Heal(healthToRestore);
                GameObject sound = Instantiate(soundPrefab);
            }
        }

        if (currencyAmount > 0)
        {
            gameManager.instance.playerScript.AddCurrency(currencyAmount);
            GameObject sound = Instantiate(soundPrefab);
            
        }

        if (magazineAmount > 0)
        {
            if (gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].isAmmoFull) 
            {
                if (gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].currentAmmo != 0)
                    StartCoroutine(gameManager.instance.MaxAmmoPopUp(0));
                return;
            }
            
            gameManager.instance.playerScript.gunPickup(gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun]);
            GameObject sound = Instantiate(soundPrefab);
            //gameManager.instance.UpdateUI();
        }

        if (throwable != null)
        {
            if (throwable == gameManager.instance.playerScript.equipment)
            {
                gameManager.instance.playerScript.equipment.currentAmount += 1;
                Destroy(gameObject);
            }
            else if (gameManager.instance.playerScript.equipment == null)
            {
                gameManager.instance.playerScript.equipment = throwable;
                throwable.currentAmount = 1;
                Destroy(gameObject);
            }
            else if (gameManager.instance.playerScript.equipment != null)
            {
                popUpActive = true;
               // equipPopUp.SetActive(true);
               gameManager.instance.popupWeaponName.text = throwable.iName;
               gameManager.instance.pickUpPopup.SetActive(true);
            }

            gameManager.instance.UpdateEquipmentUI();
        }

        if (respawnTime == 0 && !popUpActive)
            Destroy(gameObject);
    //     else
    //         StartCoroutine(HideForSeconds(respawnTime));
        }

    private IEnumerator HideForSeconds(float seconds)
    {
        ShowPickup(false);
        yield return new WaitForSeconds(seconds);
        ShowPickup(true);
    }

    private void ShowPickup(bool shouldShow)
    {
        GetComponent<Collider>().enabled = shouldShow;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //popUpActive = false;
        if (other.CompareTag("Player"))// && popUpActive == false)
        {
            popUpActive = false;
            //equipPopUp.SetActive(false);
            gameManager.instance.pickUpPopup.SetActive(false);
            
        }
    }
}
