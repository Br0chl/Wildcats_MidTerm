using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    [SerializeField] int burnAmount;
    [SerializeField] int burnCooldown;
    bool canBurn = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canBurn)
        {
            Debug.Log("player in lava stay");
            StartCoroutine(LavaDmg());
        }
    }

    IEnumerator LavaDmg()
    {
        if (canBurn)
        {
            gameManager.instance.playerScript.takeDamage(burnAmount);
            canBurn = false;
        }
        yield return new WaitForSeconds(burnCooldown);
        canBurn = true;
    }
}
