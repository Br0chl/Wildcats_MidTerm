using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigClaw : MonoBehaviour
{
    [SerializeField] CrabBoss boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(boss.slashDamage);
        }
    }
}
