using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)] [SerializeField] int HP;
    [Range(1, 10)] [SerializeField] int playerSpeed;
    [Range(5, 20)] [SerializeField] int jumpAmount;
    [Range(5, 50)] [SerializeField] int gravity;
    [Range(1, 5)] [SerializeField] int jumpsAllowed;

    int jumpedTimes;
    Vector3 move;
    Vector3 playerVelocity;

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsAllowed)
        {
            playerVelocity.y = jumpAmount;
            jumpedTimes++;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;
    }
}
