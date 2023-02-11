using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabBoss : MonoBehaviour, isDamageable
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    // Boss
    [SerializeField] SkinnedMeshRenderer bossRenderer;
    [SerializeField] SphereCollider inRangeCollider;
    [SerializeField] SphereCollider bossCollider;
    [SerializeField] GameObject attackCollider;
    [SerializeField] ParticleSystem healPS;
    public int bossHp;
    public int bossMaxHP;
    public int bossHeal;
    public float bossHealTimer;
    public Transform healPOS;
    public bool canHeal = true;
    public bool isDead = false;
    public bool bothClawsDestroyed = false;

    // BigClaw
    [SerializeField] GameObject bigClaw;
    [SerializeField] SkinnedMeshRenderer bigClawRenderer;
    [SerializeField] BoxCollider bigClawCollider;
    public int bigClawHP;
    public int bigClawStartHP;
    public bool bigClawDestroyed = false;

    // LittleClaw
    [SerializeField] GameObject littleClaw;
    [SerializeField] SkinnedMeshRenderer littleClawRenderer;
    [SerializeField] BoxCollider littleClawCollider;
    public int littleClawHP;
    public int littleClawStartHp;
    public int littleClawTimeToKill;
    public bool littleClawDestroyed = false;


    public bool inRange;

    // Slash Attack
    public bool canSlashAttack = true;
    public float slashRange;
    public int slashDamage;

    //Combo Attack BigClaw required
    public bool canCombo = true;
    public int stabDamage;

    // Start is called before the first frame update
    void Start()
    {
        inRangeCollider.enabled = true;
        bossCollider.enabled = false;
        littleClawCollider.enabled = false;
        bigClawCollider.enabled = true;
        bossHp = bossMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && !isDead)
        {
            transform.LookAt(gameManager.instance.player.transform);
            anim.SetFloat("speed", agent.velocity.normalized.magnitude);
            agent.SetDestination(gameManager.instance.player.transform.position);

            // Slash Attack
            if (agent.remainingDistance < slashRange && canSlashAttack && !bigClawDestroyed)
                StartCoroutine(SlashAttack());
            if (agent.remainingDistance < slashRange && canCombo && !bigClawDestroyed)
                StartCoroutine(Combo());
        }
        else if (!inRange && !isDead)
        {
            //healPS.Play(true);
            if (canHeal && bothClawsDestroyed)
                StartCoroutine(HealSelf());
            agent.SetDestination(transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            anim.SetBool("inRange", true);
            if (bothClawsDestroyed)
                bossCollider.enabled = true;
            if (healPOS.childCount > 0)
                Destroy(healPOS.GetChild(0).gameObject);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            bossCollider.enabled = false;
            anim.SetBool("inRange", false);
            if (bothClawsDestroyed)
                Instantiate(healPS, healPOS);
        }
    }

    // Attacks Melee Range
    IEnumerator SlashAttack()
    {
        canSlashAttack = false;
        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(2);
        attackCollider.SetActive(false);
        canSlashAttack = true;
    }

    IEnumerator Combo()
    {
        canCombo = false;
        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(.8f);
        anim.SetTrigger("Stab");
        yield return new WaitForSeconds(4);
        canCombo = true;
    }

    public void BigClawTakeDamage(int dmg)
    {
        bigClawHP -= dmg;
        StartCoroutine(flashDamage(bigClawRenderer));

        if (bigClawHP <= 0)
        {
            bigClawCollider.enabled = false;
            bigClawDestroyed = true;
            bigClaw.SetActive(false);
            StartCoroutine(LittleClawTimer());
        }
    }

    public void LittleClawTakeDamage(int dmg)
    {
        littleClawHP -= dmg;
        StartCoroutine(flashDamage(littleClawRenderer));
    }

    IEnumerator LittleClawTimer()
    {
        agent.SetDestination(transform.position);
        //agent.speed = 0;
        anim.SetBool("isRegen", true);
        littleClawCollider.enabled = true;
        yield return new WaitForSeconds(littleClawTimeToKill);
        anim.SetBool("isRegen", false);
        if (littleClawHP <= 0)
        {
            littleClawCollider.enabled = false;
            littleClawDestroyed = true;
            littleClaw.SetActive(false);
            bothClawsDestroyed = true;
            bossCollider.enabled = true;
        }
        else if (littleClawHP > 0)
        {
            littleClawCollider.enabled = false;
            littleClawHP = littleClawStartHp;
            bigClaw.SetActive(true);
            bigClawHP = bigClawStartHP;
            bigClawDestroyed = false;
            bigClawCollider.enabled = true;
        }
    }

    void DamageBoss(int dmg)
    {
        bossHp -= dmg;
        StartCoroutine(flashDamage(bossRenderer));
        
        if (bossHp <= 0)
        {
            bossCollider.enabled = false;
            inRangeCollider.enabled = false;
            agent.SetDestination(transform.position);
            agent.speed = 0;
            anim.SetTrigger("Death");
            isDead = true;           
        }
    }

    public void TakeDamage(int dmg)
    {
        if (!bigClawDestroyed)
            BigClawTakeDamage(dmg);
        else if (bigClawDestroyed && !littleClawDestroyed)
            LittleClawTakeDamage(dmg);
        else if (bigClawDestroyed && littleClawDestroyed)
            DamageBoss(dmg);
    }

    public IEnumerator flashDamage(SkinnedMeshRenderer bossPart)
    {
        bossPart.sharedMaterial.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        bossPart.sharedMaterial.color = Color.white;
    }

    IEnumerator HealSelf()
    {
        if (bossHp == bossMaxHP) yield break;
        canHeal = false;
        bossHp += bossHeal;
        yield return new WaitForSeconds(bossHealTimer);
        canHeal = true;
    }

    public void AttackColliderOn()
    {
        attackCollider.SetActive(true);
    }

    public void AttackColliderOff()
    {
        attackCollider.SetActive(false);
    }
}
