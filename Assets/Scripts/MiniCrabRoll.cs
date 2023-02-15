using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCrabRoll : MonoBehaviour
{
    public ParticleSystem part;
    [SerializeField] public List<ParticleCollisionEvent> collisionEvents;

    bool canAttack = true;
    [SerializeField] int rollDamage;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {

        
            if (other.CompareTag("Player"))
            {
                if(canAttack)
                    StartCoroutine(RollAttack());
            }
    }

    IEnumerator RollAttack()
    {
        canAttack = false;
        gameManager.instance.playerScript.takeDamage(rollDamage);
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }
}
