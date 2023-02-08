using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBullet : MonoBehaviour
{
    public int bulletDamage;
    [SerializeField] int timer;
    [Range(1,10)][SerializeField] int tickDamage;
    [Range(0.3f, 1)] [SerializeField] float burnTick;
    [Range(1, 3)] [SerializeField] int burnTime;

    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(bulletDamage);
            gameManager.instance.playerScript.TakeDamageOverTime(tickDamage, burnTime, burnTick);
        }       
        Destroy(gameObject);
    }
}
