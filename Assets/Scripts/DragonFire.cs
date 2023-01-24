using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFire : MonoBehaviour
{
    public ParticleSystem part;
    [SerializeField] public List<ParticleCollisionEvent> collisionEvents;

    bool canAttack = true;
    [SerializeField] int breathDamage;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {


        if (other.CompareTag("Player"))
        {
            if (canAttack)
                StartCoroutine(BreathAttack());
            Debug.Log("Player");
        }
    }

    IEnumerator BreathAttack()
    {
        canAttack = false;
        gameManager.instance.playerScript.takeDamage(breathDamage);
        yield return new WaitForSeconds(.05f);
        canAttack = true;
    }
}
