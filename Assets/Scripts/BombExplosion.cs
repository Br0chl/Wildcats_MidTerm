using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [Header("----- Explosion Components -----")]
    [SerializeField] enemyAI bomb;
    [SerializeField] BombType type;
    [SerializeField] ParticleSystem ignitionParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] Animator anim;

    [Header("----- Self Destruct Stats ----")]
    [Range(1, 30)][SerializeField] float explosionRadius;
    [Range(1, 5)][SerializeField] float timeTillDetonation;
    [Range(1, 20)][SerializeField] int explosionDamage;
    [SerializeField] int pushBackAmount;
    [SerializeField] bool implosion;

    [Header("----- Fire Explosion -----")]
    [Range(1, 3)][SerializeField] int burnDamage;
    [Range(0.3f, 1)][SerializeField] float burnTick;
    [Range(1, 3)][SerializeField] int burnTime;

    [Header("----- Ice Explosion -----")]
    [Range(1, 10)][SerializeField] int slowDuration;
    [Range(1, 10)][SerializeField] int overrideSpeed;

    float detonationTimer;
    bool exploding;

    public enum BombType
    {
        Base,
        Fire,
        Ice
    }

    public void HandleSelfDesturct()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        anim.SetTrigger("Exploding");
        ignitionParticles.Play();       
        yield return new WaitForSeconds(timeTillDetonation);
        ignitionParticles.Stop();
        HandleExplosion();
    }

    void HandleExplosion()
    {

        if (explosionParticles != null)
        { explosionParticles.Play(); }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                switch (type)
                {
                    case BombType.Fire:
                        FireExplosion();
                        break;

                    case BombType.Ice:
                        IceExplosion();
                        break;

                    default:
                        BaseExplosion();
                        break;
                }
            }
        }
        bomb.TakeDamage(1000);
        Destroy(gameObject);
    }

    void FireExplosion()
    {
        if (implosion)
        {
            gameManager.instance.playerScript.AssignPushback((transform.position - gameManager.instance.player.transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        else
        {
            gameManager.instance.playerScript.AssignPushback((gameManager.instance.player.transform.position - transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        gameManager.instance.playerScript.TakeDamageOverTime(burnDamage, burnTime, burnTick);
        bomb.TakeDamage(500);
    }

    void IceExplosion()
    {
        if (implosion)
        {
            gameManager.instance.playerScript.AssignPushback((transform.position - gameManager.instance.player.transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        else
        {
            gameManager.instance.playerScript.AssignPushback((gameManager.instance.player.transform.position - transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        gameManager.instance.playerScript.OverrideSpeed(overrideSpeed, slowDuration);
        bomb.TakeDamage(500);
    }

    void BaseExplosion()
    {
        if (implosion)
        {
            gameManager.instance.playerScript.AssignPushback((transform.position - gameManager.instance.player.transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        else
        {
            gameManager.instance.playerScript.AssignPushback((gameManager.instance.player.transform.position - transform.position).normalized * pushBackAmount);
            gameManager.instance.playerScript.takeDamage(explosionDamage);
        }
        bomb.TakeDamage(500);
    }
}
