using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    [SerializeField] int burnAmount;
    [SerializeField] int burnCooldown;
    bool canBurn = true;
    int burnCount = 1;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canBurn)
        {
            Debug.Log("player in lava stay");
            StartCoroutine(LavaDmg());
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
            burnCount = 1;
    }

    IEnumerator LavaDmg()
    {
        if (canBurn)
        {
            gameManager.instance.playerScript.takeDamage(burnAmount * burnCount);
            burnCount++;
            canBurn = false;
        }
        yield return new WaitForSeconds(burnCooldown);
        canBurn = true;
    }
}
