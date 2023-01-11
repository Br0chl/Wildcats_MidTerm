using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)] [SerializeField] int maxHP;
    [Range(1, 10)] [SerializeField] int walkSpeed;
    [Range(10,40)][SerializeField] int sprintSpeed;
    [Range(5, 20)] [SerializeField] int jumpAmount;
    [Range(5, 50)] [SerializeField] int gravity;
    [Range(1, 5)] [SerializeField] int jumpsAllowed;

    //**** Temporary to check UI conditions ****
    [Header("--Gun Stats---")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    //****                   *****

    [Header("----- Test View -----")]
    [SerializeField] private float playerSpeed;

    // Current HP for healthbar
    int currentHP;

    int jumpedTimes;

    public bool isShooting;
    bool isSprinting;
    bool isJumping;

    Vector3 move;
    Vector3 playerVelocity;

    private void Start() 
    {
        // Set and update player HP for UI purposes
        currentHP = maxHP;
        UpdatePlayerHP();
        RespawnPlayer();
    }

    void Update()
    {
        float delta = Time.deltaTime;

        HandleInputs();
        Movement(delta);

        if (!isShooting && Input.GetButton("Shoot"))
            StartCoroutine(Shoot());

    }

    void Movement(float delta)
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
            jumpedTimes = 0;
        }
       
        if (isSprinting)
        {
            playerSpeed = delta * sprintSpeed;
        }
        else
        {
            playerSpeed = delta * walkSpeed;
        }
        
        controller.Move(move * playerSpeed);

        if (isJumping)
        {
            playerVelocity.y = jumpAmount;
            jumpedTimes++;
        }
        
        
        playerVelocity.y -= gravity * delta;
        controller.Move(playerVelocity * delta);
    }

    //***** Temp to test UI CONDITIONS
    IEnumerator Shoot()
    {
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            if (hit.collider.GetComponent<isDamageable>() != null)
            {
                hit.collider.GetComponent<isDamageable>().TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    //*****      ********


    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdatePlayerHP();

        if (currentHP <= 0)
        {
            gameManager.instance.PlayerDead();
        }
    }

    private void UpdatePlayerHP()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    public void RespawnPlayer()
    {
        controller.enabled = false;
        currentHP = maxHP;
        UpdatePlayerHP();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    private void HandleInputs()
    {
        // move vector
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        // jump
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsAllowed)
        { isJumping = true; }
        else
        { isJumping = false; }

        // sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        { isSprinting = true; }
        else
        { isSprinting = false; }

        // shooting
       // if (!isShooting && Input.GetButton("Shoot"))
        //{ isShooting = true; }
    }
}
