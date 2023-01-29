using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniCrab : enemyAI
{
    [SerializeField] int rollDamage;
    bool isHealing;
    bool canAttack;



    void Update()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);

        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

        if (HP < 40 && !isHealing)
            StartCoroutine(HealSelf());

    }


    // public override void TakeDamage(int dmg)
    // {
    //     HP -= dmg;
    //     StartCoroutine(flashDamage());
    //     agent.SetDestination(gameManager.instance.player.transform.position);
    //     takeDamage.Invoke(dmg);
    //     if (HP <= 0)
    //     {
    //         isDead = true;
    //         gameManager.instance.updateEnemyRemaining(-1);

    //         if (!isBoss)
    //             lootDropper.DropLoot(transform.position + transform.up);
    //         else
    //             lootDropper.GetMultipleDrops(transform.position + transform.up);

    //         // StartCoroutine(gameManager.instance.playerScript.SwapTime());
    //         Destroy(gameObject);
    //     }
    // }

    IEnumerator HealSelf()
    {
        isHealing = true;
        part.Play(true);
        Heal(20);
        yield return new WaitForSeconds(5f);
        isHealing = false;
        part.Stop(true);
    }

}
