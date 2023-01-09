using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [Header("----- Spawner Values -----")]
    [SerializeField] List<Transform> spawnLocations = new List<Transform>();
    [SerializeField] List<SpawnableEnemy> enemies = new List<SpawnableEnemy>();
    [SerializeField] int waveValueMultiplier;
    [SerializeField] int waveDurationMultiplier;
    [SerializeField] int waveDuration;  

    List<GameObject> enemiesToSpawn = new List<GameObject>();
    int currWave;
    int waveValue;
    float waveTimer;
    float spawnInterval;
    float spawnTimer;

    void Start()
    {
        currWave = 1;
        GenerateWave();
    }

    void FixedUpdate()
    {
        if (spawnTimer <= 0)
        {
            HandleSpawning();
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        if (waveTimer <= 0)
        {
            GenerateWave();
        }
    }
    
    void GenerateWave()
    {
        waveValue = currWave * waveValueMultiplier;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count; // fixed time between each enemy
        waveTimer = waveDuration;
    }

    void GenerateEnemies()
    {
        // create a temporary list of enemies to generate
        List<GameObject> generatedEnemies = new List<GameObject>();

        while (waveValue > 0)
        {
            int randEnemyID = Random.Range(0, enemies.Count); // grabbing a random enemy from the list
            int randEnemyCost = enemies[randEnemyID].cost;

            if (waveValue - randEnemyCost >= 0) // if we can afford the enemy add it to the list and subtract its cost
            {
                generatedEnemies.Add(enemies[randEnemyID].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            { break; }
        }
        Debug.Log(generatedEnemies.Count);
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    void HandleSpawning()
    {
        if (enemiesToSpawn.Count > 0)
        {
            int randEnemySpawn = Random.Range(0, spawnLocations.Count);

            Instantiate(enemiesToSpawn[0], spawnLocations[randEnemySpawn].position, Quaternion.identity);
            enemiesToSpawn.RemoveAt(0);
            spawnTimer = spawnInterval;
        }
        else
        {
            waveTimer = 0;
        }
    }
}
