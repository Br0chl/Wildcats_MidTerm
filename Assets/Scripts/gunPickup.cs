using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] public GunStats gun;
    //[SerializeField] GameObject equipPopUp;

    bool popUpActive = false;
    bool newGun;

    private void Update() 
    {
        if (popUpActive && Input.GetKeyDown(KeyCode.E))
        {
            gameManager.instance.playerScript.gunPickup(gun);
            gameManager.instance.pickUpPopup.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.gunList.Count <= 1)
            {
                gameManager.instance.playerScript.gunPickup(gun);
                Destroy(gameObject);
            }
            else if (gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun] == gun && gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].isAmmoFull)
            {
                return;
            }
            else
            {
                if (gameManager.instance.playerScript.gunList.Contains(gun))
                {
                    gameManager.instance.playerScript.gunPickup(gun);
                    Destroy(gameObject);
                }
                else
                {
                    //equipPopUp.SetActive(true);
                    popUpActive = true;
                    gameManager.instance.popupWeaponName.text = gun.iName;
                    gameManager.instance.pickUpPopup.SetActive(true);
                }
                    
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            //equipPopUp.SetActive(false);
            gameManager.instance.pickUpPopup.SetActive(false);
            popUpActive = false;
        }
    }
}
