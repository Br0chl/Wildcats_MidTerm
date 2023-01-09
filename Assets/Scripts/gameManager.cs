using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public int enemiesRemaining;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;

        if (enemiesRemaining <= 0)
        {
            Debug.Log("You win!");
        }

        // check to see if game is over based on enemy count <=0
    }
}

