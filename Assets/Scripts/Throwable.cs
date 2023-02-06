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
        // else if (type == ThrowType.Knife)
        //     StartCoroutine(Throw());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime);
        
        transform.GetComponent<Rigidbody>().isKinematic = true;
        part.Play(true);

        Collider[] hits = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<enemyAI>().TakeDamage(damage);
            }
            else
            {

            }
        }

        Destroy(gameObject, 5f);
    }
}