using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActions : MonoBehaviour
{
    [Header("----- Gun Stats -----")]
    [SerializeField] public Transform shootPos;
    [SerializeField] public GameObject bullet;
    [Range(15, 35)][SerializeField] public int bulletSpeed;
    [Range(0.1f, 2)][SerializeField] public float shootRate;
    [Range(5, 100)][SerializeField] public int shootDist;
    [Range(1, 10)][SerializeField] public int shootDamage;

    public void HandleShoot()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;

        yield return new WaitForSeconds(shootRate);
        gameManager.instance.playerScript.isShooting = true;
    }
}
