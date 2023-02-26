using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [Header("----- Spawner Values -----")]
    [SerializeField] List<Transform> spawnLocations = new List<Transform>(); // stores all the spawn locations
    [SerializeField] List<SpawnableEnemy> enemies = new List<SpawnableEnemy>(); // a list of enemies and their cost
    [SerializeField] List<SpawnableEnemy> bosses = new List<SpawnableEnemy>(); // a list of bosses
    [SerializeField] int maxEnemiesInstantiated; // max amount of enemies that can be spawned at one time
    [SerializeField] int waveValueMultiplier; // the value you multiply the current wave by to get the value of the wave
    [SerializeField] int waveDuration; // the time between waves
    [SerializeField] int maxWaves; // when waves stop
    [SerializeField] int bossWaves; // set how often boss waves happen (IE: 10 == every 10 waves, 7 == every 7 waves, etc.) (0 == no bosses)
    [SerializeField] bool infiniteWaves; // sets wether the waves come infinitely

    [Header("----- Test View -----")]
    [SerializeField] List<GameObject> enemiesToSpawn = new List<GameObject>(); // the list of generated enemies to spawn
    [SerializeField] int enemiesRemaining = 0;
    [SerializeField] int currWave = 0;
    [SerializeField] int waveValue; // varriable used to buy enemies
    [SerializeField] float waveTimer;
    [SerializeField] float spawnInterval;
    [SerializeField] float spawnTimer;
    [SerializeField] bool stopWaves = true;

    [SerializeField] GameObject waveComplete;

    public bool spawnerEmpty = false;

    void Start()
    {
        gameManager.instance.SelectDifficutly();
    }

    void FixedUpdate()
    {
        if (!stopWaves) // if we still have waves to spawn
        {
            if (spawnTimer <= 0) // if its time to spawn an enemy
            { HandleSpawning(); } // spawn the enemy

            else
            {
                if (enemiesRemaining <= maxEnemiesInstantiated)
                { spawnTimer -= Time.fixedDeltaTime; } 
            }

            if (waveTimer <= 0 && enemiesRemaining <= 0) // if its time for the next wave
            { GenerateWave(); } // generate the wave

            else // otherwise adjust waveTimer
            {
                if (enemiesRemaining <= maxEnemiesInstantiated)
                { waveTimer -= Time.fixedDeltaTime; } 
            }
        }
        else if (stopWaves && enemiesRemaining == 0 && spawnerEmpty)
            StartCoroutine(WaveBreak());
        else
        {
            if (enemiesToSpawn.Count == 0 && enemiesRemaining <= 0 && currWave > 1 && currWave == maxWaves)
            { gameManager.instance.PlayerWin(); }
        }
    }
    
    void GenerateWave()
    {
        if (currWave == maxWaves && !infiniteWaves) // if we completed the last wave and we aren't playing infinitely
        {
            stopWaves = true; // stop spawning waves
            return; 
        }

        currWave++; // go to the next wave
        gameManager.instance.updateWaves(currWave);
        waveValue = currWave * waveValueMultiplier; // calculate the cost of the wave

        if (bossWaves != 0)
        {
            if (currWave % bossWaves == 0)
            { GenerateBoss(); } 
        }
        else
        { GenerateEnemies(); } // generate the list of enemies

        spawnInterval = waveDuration / enemiesToSpawn.Count; // fixed time between each enemy
        waveTimer = waveDuration; // reset the wave timer
    }

    void GenerateEnemies()
    {
        // create a temporary list of enemies to generate
        List<GameObject> generatedEnemies = new List<GameObject>();

        while (waveValue > 0) // while there is still money to spend on the wave
        {
            int randEnemyID = Random.Range(0, enemies.Count); // grabbing a random enemy from the list
            int randEnemyCost = enemies[randEnemyID].cost;   // grabbing the random enemies cost

            if (waveValue - randEnemyCost >= 0) // if we can afford the enemy add it to the list and subtract its cost
            {
                generatedEnemies.Add(enemies[randEnemyID].enemyPrefab); // add the enemy to the list of enemies
                waveValue -= randEnemyCost; // subtract the enemies cost from the waves value
            }
            else if (waveValue <= 0) // otherwise
            { break; }
        }
        enemiesToSpawn.Clear(); // make sure our enemy to spawn list is clear
        enemiesToSpawn = generatedEnemies; // copy the new waves enemies to the to spawn 
    }

    void GenerateBoss()
    {
        // create a temporary list of enemies to generate
        List<GameObject> generatedEnemies = new List<GameObject>();

        int randEnemyID = Random.Range(0, bosses.Count); // grabbing a random enemy from the list
        int randEnemyCost = bosses[randEnemyID].cost;   // grabbing the random enemies cost
        waveValue = 0;

        enemiesToSpawn.Clear(); // make sure our enemy to spawn list is clear
        enemiesToSpawn = generatedEnemies; // copy the new waves enemies to the to spawn 
        
    }

    void HandleSpawning()
    {
        if (enemiesToSpawn.Count > 0) // if we have an enemy to spawn
        {
            int randEnemySpawn = Random.Range(0, spawnLocations.Count); // choose a random location to spawn the enemy

            Instantiate(enemiesToSpawn[0], spawnLocations[randEnemySpawn].position, Quaternion.identity); // spawn the enemy
            enemiesToSpawn.RemoveAt(0); // remove the enemy from the list
            enemiesRemaining++;
            gameManager.instance.enemiesRemainingText.text = enemiesRemaining.ToString("F0");
            spawnTimer = spawnInterval; // reset the spawnTimer
        }
        else // otherwise
        {
            spawnerEmpty = true;
            stopWaves = true;
            waveTimer = 0; // end the wave
            
        }
    }

    public void StartSpawner(int wavesAmount, int wavesMultiplier)
    {
        maxWaves = wavesAmount;
        gameManager.instance.maxWaves = wavesAmount;
        waveValueMultiplier = wavesMultiplier;
        stopWaves = false;
        gameManager.instance.UnPause();
        GenerateWave();
    }

    public void updateEnemyReamining(int amount)
    {
        enemiesRemaining += amount;
        gameManager.instance.enemiesRemainingText.text = enemiesRemaining.ToString("F0");
    }

    IEnumerator WaveBreak()
    {
        Debug.Log("Wave Complete");
        waveComplete.SetActive(true);
        yield return new WaitForSeconds(5);
        waveComplete.SetActive(false);
        Debug.Log("starting next wave");
        spawnerEmpty = false;
        stopWaves = false;
    }
}
