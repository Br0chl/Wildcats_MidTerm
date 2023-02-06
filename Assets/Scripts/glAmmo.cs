using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glAmmo : MonoBehaviour
{
    public Rigidbody projectileRb;
    public ParticleSystem part;
    int damage;

    
    void Start()
    {
        damage = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].shootDamage;

        // Calculate the direction from throw position to aim position
        Vector3 forceDirection = Camera.main.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 500f))
        {
            forceDirection = (hit.point - transform.position).normalized;
        }

        Rigidbody projectileRb = GetComponent<Rigidbody>();
        // Add force
        Vector3 forceToAdd = forceDirection * 100f;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    void Explode()
    {
        gameObject.transform.parent = null;
        projectileRb.isKinematic = true;
        part.Play(true);

        Collider[] hits = Physics.OverlapSphere(transform.position, 10f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.GetType() == typeof(CapsuleCollider))
            {
                hit.GetComponent<enemyAI>().TakeDamage(damage);
            }
            else
            {

            }
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy") && other.GetType() == typeof(CapsuleCollider) || other.CompareTag("Untagged"))
        {
            Explode();
        }
    }
}
