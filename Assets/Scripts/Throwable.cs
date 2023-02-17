using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThrowType
{
    Grenade,
    Knife
}

public class Throwable : MonoBehaviour
{
    [SerializeField] public ThrowType type;
    [SerializeField] public string iName;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audExplosion;
    [Range(0, 1)][SerializeField] float audExplosionVol;
    public Sprite icon;
    public ParticleSystem part;
    public int explodeTime;
    public int explodeRadius;
    public int damage;

    public int currentAmount;

    public float throwForce;
    public float throwUpwardForce;

    public float throwCooldown;

    void Start()
    {
        if (type == ThrowType.Grenade)
            StartCoroutine(Explode());
        if (type == ThrowType.Knife)
        {
            aud.PlayOneShot(audExplosion, audExplosionVol);
            Destroy(gameObject, 3f);
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime);
        
        transform.GetComponent<Rigidbody>().isKinematic = true;
        aud.PlayOneShot(audExplosion, audExplosionVol);
        part.Play(true);

        Collider[] hits = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.GetType() == typeof(CapsuleCollider))
            {
                hit.GetComponent<enemyAI>().TakeDamage(damage);
            }
            else if (hit.CompareTag("Player"))
            {
                gameManager.instance.playerScript.takeDamage(damage);
            }
        }

        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If damage powerup is active
        if (other.CompareTag("Enemy") && other.GetType() == typeof(CapsuleCollider))
        {
            other.GetComponent<enemyAI>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
