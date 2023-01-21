using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] int healthToRestore;
    [SerializeField] int currencyAmount;

    [SerializeField] float respawnTime = 0f;

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
                gameManager.instance.playerScript.Heal(healthToRestore);
        }

        if (currencyAmount > 0)
        {
            gameManager.instance.playerScript.AddCurrency(currencyAmount);
        }

        if (respawnTime == 0)
            Destroy(gameObject);
        else
            StartCoroutine(HideForSeconds(respawnTime));
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
}
