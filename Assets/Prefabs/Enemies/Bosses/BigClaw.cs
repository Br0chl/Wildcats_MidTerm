using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigClaw : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(50);
        }
    }
}
