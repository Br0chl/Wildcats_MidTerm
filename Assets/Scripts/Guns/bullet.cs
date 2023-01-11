using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [HideInInspector] public int bulletDamage;
    [SerializeField] int timer;

    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.TakeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
}
