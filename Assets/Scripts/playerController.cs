using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)] [SerializeField] int HP;
    [Range(1, 10)] [SerializeField] int walkSpeed;
    [Range(10,40)][SerializeField] int sprintSpeed;
    [Range(5, 20)] [SerializeField] int jumpAmount;
    [Range(5, 50)] [SerializeField] int gravity;
    [Range(1, 5)] [SerializeField] int jumpsAllowed;

    [Header("----- Test View -----")]
    [SerializeField] private float playerSpeed;

    int jumpedTimes;

    bool isSprinting;
    bool isJumping;

    Vector3 move;
    Vector3 playerVelocity;

    void Update()
    {
        float delta = Time.deltaTime;

        HandleInputs();
        Movement(delta);
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

    public void TakeDamage(int dmg)
    {
        HP -= dmg;
    }

    private void HandleInputs()
    {
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsAllowed)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }
}
