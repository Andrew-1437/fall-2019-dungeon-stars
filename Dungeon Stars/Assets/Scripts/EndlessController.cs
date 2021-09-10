using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessController : MonoBehaviour
{
    float startTime = -1f;

    [Header("Endless Mode variables")]
    public int difficultyLevel = 0;
    public float timeBetweenDifficultyIncrease; // Time interval at the end of which the difficulty will increase
                                                // Set this to something high, preferably >30 sec

    [Header("Spawn lists")]
    public GameObject[] spawnList;  // List of normal enemies to spawn. Sorted from Easy -> Hard
    public GameObject[] dangerSpawnList;   // List of more dangerous/complex enemies to spawn. Sorted from Easy -> Hard
    public GameObject[] powerUpList;    // List of power ups to spawn. Index 0 should be LevelUp
    public GameObject hpRepairPowerUp;  // Hp repair power up to spawn after a boss
    public GameObject[] bossesList; // List of bosses to spawn in order of appearance (or random maybe?)

    public bool spawnEnemies;
    public bool activeBoss;

    [Header("Spawn timing variables")]
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

    [Header("References")]
    public Fungus.Flowchart flowchart;

    private IEnumerator endlessMode;

    public delegate void EndlessDelegate(int difficulty);
    public static event EndlessDelegate OnEndlessDifficultyIncrease;

    // Start is called before the first frame update
    void Start()
    {
        StartEndlessTimer();
        endlessMode = EndlessDifficultyIncrease();

        BossBehavior.OnBossDeath += BossBehavior_OnBossDeath;
        PlayerController.OnPlayerDeath += PlayerController_OnPlayerDeath;
        GM.OnLevelComplete += GM_OnLevelComplete;
        GM.OnExitToMainMenu += GM_OnExitToMainMenu;
    }

    // When player dies
    private void PlayerController_OnPlayerDeath(PlayerController pc)
    {
        throw new System.NotImplementedException();
    }

    // When boss is killed
    private void BossBehavior_OnBossDeath()
    {
        activeBoss = false;
        spawnEnemies = true;
        IncreaseDifficulty();

        // Spawn a repair power up that restores 75% of missing hp and shield
        Instantiate(hpRepairPowerUp, transform.position + Vector3.up * 20f, transform.rotation);

        // Increase difficulty modifiers after boss is defeated
        OmniController.omniController.additionalScoreMultiplier += .15f;
        OmniController.omniController.obstacleHpScale += .15f;
        OmniController.omniController.obstacleSpeedScale += .15f;
        OmniController.omniController.playerIncommingDamageScale += .05f;
        OmniController.omniController.enemyFireRateScale *= .9f;

        // Increase Time Between Diffiiculty Increase
        timeBetweenDifficultyIncrease += 1;
    }

    // When the level is completed
    private void GM_OnLevelComplete()
    {
        StopCoroutine(EndlessDifficultyIncrease());
        OmniController.omniController.finalDifficultyLevel = difficultyLevel;
        OmniController.omniController.timeTaken = TimeSurvived();

        // Unsubscribe all subscribers
        UnsubAllEvents();
    }

    private void GM_OnExitToMainMenu()
    {
        StopCoroutine(EndlessDifficultyIncrease());

        // Unsubscribe all subscribers
        UnsubAllEvents();
    }

    // Update is called once per frame
    void Update()
    {
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

    // Spawns the next boss from the list and links it to the GM
    public void SpawnBoss()
    {
        GM.gameController.boss =
            Instantiate(
                bossesList[(difficultyLevel / 6 - 1) % bossesList.Length]);
        GM.gameController.AwakenBoss();
    }

    public void IncreaseDifficulty()
    {
        difficultyLevel++;
        OnEndlessDifficultyIncrease?.Invoke(difficultyLevel);
        minTimeBetweenGroups = minTimeBetweenGroups * .98f;
        maxTimeBetweenGroups = maxTimeBetweenGroups * .98f;

        if (difficultyLevel > 3)
        {
            minTimeBetweenDangerGroups = minTimeBetweenDangerGroups * .98f;
            maxTimeBetweenDangerGroups = maxTimeBetweenDangerGroups * .98f;
        }

        // Every 6 levels of difficulty, spawn a boss, stop enemies from spawning, and stop difficulty from increasing
        if (difficultyLevel % 6 == 0)
        {
            activeBoss = true;
            spawnEnemies = false;
            flowchart.SendFungusMessage("boss");
        }
    }

    private void UnsubAllEvents()
    {
        BossBehavior.OnBossDeath -= BossBehavior_OnBossDeath;
        PlayerController.OnPlayerDeath -= PlayerController_OnPlayerDeath;
        GM.OnLevelComplete -= GM_OnLevelComplete;
        GM.OnExitToMainMenu -= GM_OnExitToMainMenu;
    }

    IEnumerator EndlessDifficultyIncrease()
    {
        for(; ; )
        {
            if(activeBoss)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            yield return new WaitForSeconds(timeBetweenDifficultyIncrease); // Every interval, increase the difficulty slightly

            IncreaseDifficulty();

            
        }
    }
}
