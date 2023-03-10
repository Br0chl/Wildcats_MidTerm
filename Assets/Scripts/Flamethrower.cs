using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    public bool canAttack = true;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnDisable() 
    {
        canAttack = true;
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        isDamageable dmg = other.GetComponent<isDamageable>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (dmg != null && canAttack)
            {
                dmg.TakeDamage(gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].shootDamage);
                StartCoroutine(FlameAttack());
            }
            i++;
        }
    }

    IEnumerator FlameAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(.1f);
        canAttack = true;
    }
}
