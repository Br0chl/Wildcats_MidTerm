using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class enemyAI : MonoBehaviour, isDamageable
{
    [Header("-----Components-----")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator anim;
    [SerializeField] protected ParticleSystem part;
    [SerializeField] SkinnedMeshRenderer model;
    [SerializeField] protected UnityEvent<int> takeDamage;

    [Header("-----Enemy Stats-----")]
    [SerializeField] Enemies enemyType;
    [SerializeField] Transform headPos;
    [Range(0, 200)] [SerializeField] protected int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewAngle;
    [SerializeField] int shootAngle;

    [Header("-----Enemy Stats-----")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [Range(15, 35)] [SerializeField] int bulletSpeed;
    [Range(0.1f, 2)] [SerializeField] float shootRate;
    [Range(5, 100)] [SerializeField] int shootDist;
    [Range(1, 10)] [SerializeField] int shootDamage;

    [Header("-----Loot Dropper-----")]
    [SerializeField] protected LootDropper lootDropper;

    [Header("----- Bomb Only -----")]
    [SerializeField] BombExplosion bombHandler;
    [Range(1, 5)][SerializeField] int selfDestructStartRange;

    float angleToPlayer;

    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;

    protected bool isBoss;
    protected bool isDead = false;

    GameObject bulletClone;

    public enum Enemies
    {
        Crab,
        Dragon,
        Blossom,
        Bomb
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateEnemyRemaining(1);
        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        if (playerInRange)
        {
            canSeePlayer();
        }
    }

    private void canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    facePlayer();
                }

                if (!isShooting && angleToPlayer <= shootAngle && !isDead)
                {
                    StartCoroutine(shoot());
                }

                if (Vector3.Distance(transform.position, gameManager.instance.player.transform.position) < selfDestructStartRange && enemyType == Enemies.Bomb)
                {
                    bombHandler.HandleSelfDesturct(); 
                }
            }
        }
    }

    public virtual void TakeDamage(int dmg)
    {
        HP -= dmg;
        anim.SetTrigger("Damage");
        agent.SetDestination(gameManager.instance.player.transform.position);
        takeDamage.Invoke(dmg);
        if (HP <= 0)
        {
            anim.SetTrigger("Die");
            isDead = true;            
            agent.baseOffset = 0;
            gameManager.instance.updateEnemyRemaining(-1);

            // Drop Loot
            if (!isBoss)
                lootDropper.DropLoot(transform.position + transform.up);
            else
                lootDropper.GetMultipleDrops(transform.position + transform.up);

            Destroy(gameObject);
        }
    }

    public IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;
    }

    // uncommented for shooting
    IEnumerator shoot()
    {
       isShooting = true;
    

        anim.SetTrigger("Shoot");
        switch (enemyType)
        {
            case Enemies.Crab:
                bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().velocity = playerDir.normalized * bulletSpeed;
                bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
                yield return new WaitForSeconds(shootRate);
                break;
            case Enemies.Dragon:
                if (playerInRange)
                {
                    part.Play(true);
                }                        
                break;
            case Enemies.Blossom:
                bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().velocity = playerDir.normalized * bulletSpeed;
                bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
                yield return new WaitForSeconds(shootRate);
                break;
            case Enemies.Bomb:
                bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().velocity = playerDir.normalized * bulletSpeed;
                bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
                yield return new WaitForSeconds(shootRate);
                break;
            default:
                break;
        }
      
       isShooting = false;
    }

    void facePlayer()
    {
        //playerDir.y = 0;
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
            if (enemyType == Enemies.Dragon)
            {
                part.Stop(true);
            }
        }
    }

    public void Heal(int amountToHeal)
    {
        HP += amountToHeal;
    }
}
