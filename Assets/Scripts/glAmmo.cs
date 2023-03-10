using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glAmmo : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip explosionSound;
    [Range(0, 1)][SerializeField] float explosionVol;

    public Rigidbody projectileRb;
    public ParticleSystem part;
    int damage;
    public float ammoSpeed;

    
    void Start()
    {
        transform.SetParent(null, true);
        damage = gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].shootDamage;

        // Calculate the direction from throw position to aim position
        Vector3 forceDirection = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 50f))
        {
            forceDirection = (hit.point - transform.position).normalized;
        }

        Rigidbody projectileRb = GetComponent<Rigidbody>();
        // Add force
        Vector3 forceToAdd = forceDirection * ammoSpeed;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    void Explode()
    {
        // gameObject.transform.parent = null;
        // projectileRb.useGravity = false;
        // projectileRb.isKinematic = true;
        aud.PlayOneShot(explosionSound, explosionVol);
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
            gameObject.transform.parent = null;
            projectileRb.useGravity = false;
            projectileRb.isKinematic = true;
            Explode();
        }
    }
}
