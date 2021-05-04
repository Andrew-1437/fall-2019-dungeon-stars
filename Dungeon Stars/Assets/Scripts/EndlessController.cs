using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessController : MonoBehaviour
{
    float startTime = -1f;
    public int difficultyLevel = 0;

    public float timeBetweenDifficultyIncrease; // Time interval at the end of which the difficulty will increase
                                                // Set this to something high, preferably >30 sec


    public GameObject[] spawnList;  // List of normal enemies to spawn. Sorted from Easy -> Hard
    public GameObject[] dangerSpawnList;   // List of more dangerous/complex enemies to spawn. Sorted from Easy -> Hard
    public GameObject[] powerUpList;    // List of power ups to spawn. Index 0 should be LevelUp

    public bool spawnEnemies;

    public float minTimeBetweenGroups;  // Time intervals to spawn an enemy group
    public float maxTimeBetweenGroups;  // TODO: Increase difficulty with time
    float timeForNextGroup;

    public float minTimeBetweenDangerGroups;    // Time interval to spawn a dangerous group
    public float maxTimeBetweenDangerGroups;    // TODO: Increase difficulty with time
    float timeForNextDangerGroup;

    public float minTimeBetweenPowerUps;    // Time interval to spawn a random power up
    public float maxTimeBetweenPowerUps;
    float timeForNextPowerUp;

    public float timeBetweenLevelUp;    // Time interval to spawn a level up power up
    float timeForLevelUp;

    private IEnumerator endlessMode;


    // Start is called before the first frame update
    void Start()
    {
        endlessMode = EndlessDifficultyIncrease();
    }

    // Update is called once per frame
    void Update()
    {
        if(startTime == -1f && GM.gameController.gameStart)
        {
            StartEndlessTimer();
        }

        if(GM.gameController.gameStart)
        {
            if(spawnEnemies && Time.time >= timeForNextGroup)
            {
                SpawnGroup();
                timeForNextGroup = Time.time + Random.Range(minTimeBetweenGroups, maxTimeBetweenGroups);
            }
            if (spawnEnemies && difficultyLevel > 2 && Time.time >= timeForNextDangerGroup)
            {
                SpawnDangerGroup();
                timeForNextDangerGroup = Time.time + Random.Range(minTimeBetweenDangerGroups, maxTimeBetweenDangerGroups);
            }

            if (Time.time >= timeForNextPowerUp)
            {
                SpawnPowerUp();
                timeForNextPowerUp = Time.time + Random.Range(minTimeBetweenPowerUps, maxTimeBetweenPowerUps);
            }

            if (Time.time >= timeForLevelUp)
            {
                SpawnLevelUp();
                timeForLevelUp = Time.time + timeBetweenLevelUp;
            }


        }
    }

    public void StartEndlessTimer()
    {
        startTime = Time.time;
        timeForNextGroup = minTimeBetweenGroups;
        timeForNextDangerGroup = minTimeBetweenDangerGroups * 1.5f;
        timeForNextPowerUp = minTimeBetweenPowerUps;
        timeForLevelUp = timeBetweenLevelUp;
        StartCoroutine(endlessMode);
    }

    public float TimeSurvived()
    {
        return Time.time - startTime;
    }

    // Spawns a random enemy group from the list
    public void SpawnGroup()
    {
        Destroy(
            Instantiate(
                spawnList[Random.Range(0, spawnList.Length)],
                transform.position + Vector3.up * 20f + Vector3.right * Random.Range(-15f, 15f),
                transform.rotation), 60f);
    }
    public void SpawnDangerGroup()
    {
        Destroy(
            Instantiate(
                dangerSpawnList[Random.Range(0, dangerSpawnList.Length)],
                transform.position + Vector3.up * 20f + Vector3.right * Random.Range(-10f, 10f),
                transform.rotation), 60f);
    }

    // Spawns a random power up (not including the LevelUp power up)
    public void SpawnPowerUp()
    {
        Instantiate(
                powerUpList[Random.Range(1, powerUpList.Length)],
                transform.position + Vector3.up * 20f + Vector3.right * Random.Range(-20f, 20f),
                transform.rotation);
    }

    // Spawns a LevelUp power up
    public void SpawnLevelUp()
    {
        Instantiate(
                powerUpList[0],
                transform.position + Vector3.up * 20f + Vector3.right * Random.Range(-20f, 20f),
                transform.rotation);
    }

    IEnumerator EndlessDifficultyIncrease()
    {
        for(; ; )
        {
            yield return new WaitForSeconds(timeBetweenDifficultyIncrease); // Every interval, increase the difficulty slightly

            difficultyLevel++;
            minTimeBetweenGroups = minTimeBetweenGroups * .95f;
            maxTimeBetweenGroups = maxTimeBetweenGroups * .95f;

            if(difficultyLevel > 3)
            {
                minTimeBetweenDangerGroups = minTimeBetweenDangerGroups * .95f;
                maxTimeBetweenDangerGroups = maxTimeBetweenDangerGroups * .95f;
            }

            // TODO: Every 10 levels or so, spawn a boss and stop difficulty from increasing until the boss is defeated

            // Every 10 levels of difficulty, increase the total score gain and the max hp of enemies
            if(difficultyLevel % 10 == 0)
            {
                OmniController.omniController.additionalScoreMultiplier += .2f;
                OmniController.omniController.obstacleHpScale += .2f;
                OmniController.omniController.obstacleSpeedScale += .2f;
            }
        }
    }
}
