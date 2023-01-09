using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, isDamageable
{
    [Header("-----Components-----")]
    [SerializeField] NavMeshAgent agent;

    [Header("-----Enemy Stats-----")]
    [SerializeField] Transform headPos;
    [Range(0, 15)] [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;

    [Header("-----Enemy Stats-----")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [Range(15, 35)] [SerializeField] int bulletSpeed;
    [Range(0.1f, 2)] [SerializeField] float shootRate;
    [Range(5, 100)] [SerializeField] int shootDist;
    [Range(1, 10)] [SerializeField] int shootDamage;

    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateEnemyRemaining(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;

            agent.SetDestination(gameManager.instance.player.transform.position);

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                facePlayer();
            }

            //if (!isShooting)
            //{
            //    StartCoroutine(shoot());
            //}
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            gameManager.instance.updateEnemyRemaining(-1);
            Destroy(gameObject);
        }
    }

    //IEnumerator shoot()
    //{
    //    isShooting = true;

    //    GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
    //    bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    //    bulletClone.GetComponent<Bullet>().bulletDamage = shootDamage;

    //    yield return new WaitForSeconds(shootRate);
    //    isShooting = false;
    //}

    void facePlayer()
    {
        playerDir.y = 0;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
