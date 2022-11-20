using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GM : MonoBehaviour {

    #region Constants
    // Default bounds for the level
    const float LEVEL_UPPER_BOUND = 12.6f;
    const float LEVEL_LOWER_BOUND = -12.6f;
    const float LEVEL_RIGHT_BOUND = 27f;
    const float LEVEL_LEFT_BOUND = -27f;
    #endregion

    // Singleton Instance
    [HideInInspector]
    public static GM gameController;

    #region References
    [Header("References")]
    public GameObject playerObject;
    public GameObject playerObject2;
    public GameObject fx;
    public GameObject player;
    public GameObject player2;
    public PlayerController playerController;
    public PlayerController playerController2;
    public GameObject enemies;
    public GameObject ui;
    public GameObject boss;
    public GameObject bossWarnUI;
    private BossBehavior bossStats;
    private float bossMaxHp;
    #endregion

    #region Score Tracking
    [Header("Score")]
    public int score;
    public int baseAmmoScore;  // Highest score bonus if have max ammo 
    public int baseHpScore;  // Highest score bonus if have max hp
    public int scoreMultiplier = 1;
    public int chainedKills = 0;
    public float multiplierTimer;   // If time between kills exceeds this, the multiplier is reset
    float endMultiplierTime = 0f;
    #endregion

    #region Level Management
    [Header("Level Management")]
    public int levelIndex;  // Index of the level starting from 1 (1 is Level 1 and so on)
    public bool gameStart;  // Is game in progress?
    public bool warpDrive;  // Is the Warp Drive enabled in this level? [WIP]
    public bool allowBoss;
    public bool endLevelOnBossDeath;    // Do we end the level when the boss dies?
    public int playerLives; // Number of lives player has
    private bool initialSpawn1 = true;
    private bool initialSpawn2 = true;
    public bool twoPlayerMode;
    [HideInInspector]
    public bool gamePaused = false;
    bool hasDied = false;   // True if the player has died at least once this level

    [Header("Level Boundaries")]
    public float upperBounds = LEVEL_UPPER_BOUND;
    public float lowerBounds = LEVEL_LOWER_BOUND;
    public float rightBounds = LEVEL_RIGHT_BOUND;
    public float leftBounds = LEVEL_LEFT_BOUND;
    #endregion

    #region UI
    [Header("UI")]
    public GameObject soloUIElements;
    public TextMeshProUGUI health;
    public TextMeshProUGUI shields;
    public TextMeshProUGUI level;
    public TextMeshProUGUI lives;
    public SimpleHealthBar hp;
    public SimpleHealthBar shield;
    public TextMeshProUGUI bossTitle;
    public SimpleHealthBar bossHp;
    public TextMeshProUGUI missileCount;
    public GameObject heatBar;
    public SimpleHealthBar heat;
    public TextMeshProUGUI scores;
    public TextMeshProUGUI scoreMultiplierDisp;
    public Animator scoreMultiplierAnim;
    public TextMeshProUGUI baseScore;
    public TextMeshProUGUI ammoScore;
    public TextMeshProUGUI hullScore;
    public TextMeshProUGUI totalScore;
    public TMP_ColorGradient shieldOnColor;
    public TMP_ColorGradient shieldOffColor;
    public GameObject pauseMenu;

    [Header("Two Player UI")]
    public GameObject duoUIElements;
    [Header("Player 1")]
    public TextMeshProUGUI health1;
    public TextMeshProUGUI shields1;
    public TextMeshProUGUI level1;
    public SimpleHealthBar hp1;
    public SimpleHealthBar shield1;
    public TextMeshProUGUI missileCount1;
    public GameObject heatBar1;
    public SimpleHealthBar heat1;
    [Header("Player 2")]
    public TextMeshProUGUI health2;
    public TextMeshProUGUI shields2;
    public TextMeshProUGUI level2;
    public SimpleHealthBar hp2;
    public SimpleHealthBar shield2;
    public TextMeshProUGUI missileCount2;
    public GameObject heatBar2;
    public SimpleHealthBar heat2;
    #endregion

    [Header("Flowchart")]
    public Fungus.Flowchart mainFlowchart;

    #region Events
    public delegate void GmDelegate();
    public static event GmDelegate OnBossActivate;  // Invoke when Boss is awakened
    public static event GmDelegate OnLevelEnd;  // Invoked when the level is stopped
    public static event GmDelegate OnLevelComplete; // Invoked when the level is completed by reaching the end
    public static event GmDelegate OnExitToMainMenu;    // Invoked when the player leaves to the main menu
    #endregion

    private void Awake()
    {
        twoPlayerMode = OmniController.omniController.twoPlayerMode;

        // If a ship has been selected, spawn that one. Otherwise spawn the one in the GM inspector
        if (OmniController.omniController.selectedShip)
        {
            playerObject = OmniController.omniController.selectedShip;
            if (twoPlayerMode)
                playerObject2 = OmniController.omniController.selectedShip2;
        }
        if (boss)
        {
            bossStats = boss.GetComponent<BossBehavior>();
            bossMaxHp = bossStats.hp;
        }
        if (gameController != null && gameController != this)
        {
            Destroy(gameController.gameObject);
        }
        gameController = this;

        if (OmniController.omniController.infiniteLives)
            playerLives = int.MaxValue;

        soloUIElements.SetActive(!twoPlayerMode);
        duoUIElements.SetActive(twoPlayerMode);

        // If Playing with two players, double the score for hull and ammo
        if(twoPlayerMode)
        {
            baseAmmoScore *= 2;
            baseHpScore *= 2;
        }

        scoreMultiplier = 1;

        // Subscribe to events
        PlayerController.OnPlayerDeath += PlayerController_OnPlayerDeath;
        GameStarter.OnGameStart += GameStarter_OnGameStart;
        BossBehavior.OnBossDeath += BossBehavior_OnBossDeath;
    }

    private void GameStarter_OnGameStart()
    {
        gameStart = true;
        SpawnPlayer();
        if (twoPlayerMode)
            SpawnPlayer2();
        GameStarter.OnGameStart -= GameStarter_OnGameStart;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Pause"))
            PauseGame(!gamePaused);

        //Debug*************
        if (OmniController.omniController.enableDebug)
        {
            //Quick exit to menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }

            //Toggle Enemies
            if (Input.GetKeyDown("1"))
            {
                enemies.SetActive(!enemies.activeSelf);
                if (enemies.activeSelf)
                {
                    print("Enemies enabled");
                }
                else
                {
                    print("Enemies disabled");
                }
            }
            //Toggle GUI Elements
            if (Input.GetKeyDown("2"))
            {
                ui.SetActive(!ui.activeSelf);
                if (ui.activeSelf)
                {
                    print("UI enabled");
                }
                else
                {
                    print("UI disabled");
                }
            }
            //Spawns another Player
            if (Input.GetKeyDown("0"))
            {
                print("Respawning Player...");
                player = GameObject.FindWithTag("Player");
                if (player != null)
                    player.GetComponent<PlayerController>().Die();
                if (twoPlayerMode && player2 != null)
                    player2.GetComponent<PlayerController>().Die();
                SpawnPlayer();
                if (twoPlayerMode)
                    SpawnPlayer2();

            }
            //Spawns another Player without destroying the original (for the lolz)
            if (Input.GetKeyDown("9"))
            {
                print("Spawning new Player...");
                //player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    //player.GetComponent<PlayerController>().Die();
                }
                SpawnPlayer();
                //player = GameObject.FindWithTag("Player");
            }
            // Ends the level
            if (Input.GetKeyDown(KeyCode.Equals))
                EndLevel();

            //Summons Boss
            if (Input.GetKeyDown("b") && allowBoss)
            {
                AwakenBoss();
            }
        }

        // SCORE MULTIPLIER UPDATES ==========
        if (scoreMultiplier > 1 && Time.time > endMultiplierTime)
        {
            ResetMultiplier();
        }


        //GUI Updates
        lives.text = "Lives: " + playerLives;
        scores.text = "Score: " + score.ToString();
        scoreMultiplierDisp.text = "x" + scoreMultiplier.ToString();
        if (scoreMultiplier <= 1)   // Only show multiplier if bonus is greater than 1
            scoreMultiplierDisp.enabled = false;
        else
            scoreMultiplierDisp.enabled = true;

        // ONE PLAYER ===================
        if (!twoPlayerMode)
        {
            if (player != null)
            {
                // Updates health bar's text
                health.text = "Health: " + Mathf.FloorToInt(playerController.hp) + "/" + Mathf.FloorToInt(playerController.maxHp);
                if(playerController.hp <= 0)
                    health.text = "Health: ==CRITICAL FAILURE==";
                if (playerController.hp < .3f * playerController.maxHp)
                {
                    Color color = new Color(0.9f, 0f, 0f);
                    hp.UpdateColor(color);
                }
                else
                {
                    Color color = new Color(0f, 1f, 44.0f / 255.0f);
                    hp.UpdateColor(color);
                }

                // Updates shield bar's text
                if (playerController.shield > 0.0f)
                {
                    shields.text = "Shields: " + Mathf.FloorToInt(playerController.shield) + "/" + Mathf.FloorToInt(playerController.maxShield);
                    shields.colorGradientPreset = shieldOnColor;
                }
                else
                {
                    shields.text = "Shields: =OFFLINE=";
                    shields.colorGradientPreset = shieldOffColor;
                }

                // Updates power level text (with a special case for the Meme ship)
                if (playerController.id != ShipsEnum.ShipID.MEME)
                    level.text = "Power: " + (playerController.level + 1);
                else
                    level.text = "Power: 69";

                missileCount.text = "Ammo: " + playerController.currentMissileCount + "/" + playerController.maxMissile;

                // Updates bars
                hp.UpdateBar(playerController.hp, playerController.maxHp);
                shield.UpdateBar(playerController.shield, playerController.maxShield);
                heatBar.SetActive(playerController.enableHeat);
                if (playerController.enableHeat)
                {
                    heat.UpdateBar(playerController.heat, 100f);
                    Color heatColor = new Color(1f, 230f / 255f, 0f);
                    if (playerController.overheating)
                    {
                        heatColor = new Color(1f, 30f / 255f, 0f);
                    }
                    heat.UpdateColor(heatColor);
                }
            }
            else
            {
                health.text = "Health: =NULL=";
                shields.text = "Shields: =NULL=";
                level.text = "Power: 0";
            }
        }
        // TWO PLAYER ===================
        else
        {
            if (player != null)
            {
                health1.text = "Health: " + Mathf.FloorToInt(playerController.hp) + "/" + Mathf.FloorToInt(playerController.maxHp);
                if (playerController.hp <= 0)
                    health1.text = "Health: ==CRITICAL FAILURE==";
                if (playerController.hp < .3f * playerController.maxHp)
                {
                    Color color = new Color(0.9f, 0f, 0f);
                    hp1.UpdateColor(color);
                }
                else
                {
                    Color color = new Color(0f, 1f, 44.0f / 255.0f);
                    hp1.UpdateColor(color);
                }

                if (playerController.shield > 0.0f)
                {
                    shields1.text = "Shields: " + Mathf.FloorToInt(playerController.shield) + "/" + Mathf.FloorToInt(playerController.maxShield);
                    shields1.colorGradientPreset = shieldOnColor;
                }
                else
                {
                    shields1.text = "Shields: =OFFLINE=";
                    shields1.colorGradientPreset = shieldOffColor;
                }

                if (playerController.id != ShipsEnum.ShipID.MEME)
                    level1.text = "Power: " + (playerController.level + 1);
                else
                    level1.text = "Power: 69";

                missileCount1.text = "Ammo: " + playerController.currentMissileCount + "/" + playerController.maxMissile;

                hp1.UpdateBar(playerController.hp, playerController.maxHp);
                shield1.UpdateBar(playerController.shield, playerController.maxShield);
                heatBar1.SetActive(playerController.enableHeat);
                if (playerController.enableHeat)
                {
                    heat1.UpdateBar(playerController.heat, 100f);
                    Color heatColor = new Color(1f, 230f / 255f, 0f);
                    if (playerController.overheating)
                    {
                        heatColor = new Color(1f, 30f / 255f, 0f);
                    }
                    heat1.UpdateColor(heatColor);
                }
            }
            else
            {
                health1.text = "Health: =NULL=";
                shields1.text = "Shields: =NULL=";
                level1.text = "Power: 0";
            }
            if (player2 != null)
            {
                health2.text = "Health: " + Mathf.FloorToInt(playerController2.hp) + "/" + Mathf.FloorToInt(playerController2.maxHp);
                if (playerController2.hp <= 0)
                    health2.text = "Health: ==CRITICAL FAILURE==";
                if (playerController2.hp < .3f * playerController2.maxHp)
                {
                    Color color = new Color(0.9f, 0f, 0f);
                    hp2.UpdateColor(color);
                }
                else
                {
                    Color color = new Color(0f, 1f, 44.0f / 255.0f);
                    hp2.UpdateColor(color);
                }

                if (playerController2.shield > 0.0f)
                {
                    shields2.text = "Shields: " + Mathf.FloorToInt(playerController2.shield) + "/" + Mathf.FloorToInt(playerController2.maxShield);
                    shields2.colorGradientPreset = shieldOnColor;
                }
                else
                {
                    shields2.text = "Shields: =OFFLINE=";
                    shields2.colorGradientPreset = shieldOffColor;
                }

                if (playerController2.id != ShipsEnum.ShipID.MEME)
                    level2.text = "Power: " + (playerController2.level + 1);
                else
                    level2.text = "Power: 69";

                missileCount2.text = "Ammo: " + playerController2.currentMissileCount + "/" + playerController2.maxMissile;

                hp2.UpdateBar(playerController2.hp, playerController2.maxHp);
                shield2.UpdateBar(playerController2.shield, playerController2.maxShield);
                heatBar2.SetActive(playerController2.enableHeat);
                if (playerController2.enableHeat)
                {
                    heat2.UpdateBar(playerController2.heat, 100f);
                    Color heatColor = new Color(1f, 230f / 255f, 0f);
                    if (playerController2.overheating)
                    {
                        heatColor = new Color(1f, 30f / 255f, 0f);
                    }
                    heat2.UpdateColor(heatColor);
                }
            }
            else
            {
                health2.text = "Health: =NULL=";
                shields2.text = "Shields: =NULL=";
                level2.text = "Power: 0";
            }
        }

        if (score<0)
        {
            score = 0;
        }
        
    }

    public void AwakenBoss()
    {
        OnBossActivate?.Invoke();
        bossWarnUI.GetComponent<FlashUI>().Flash();
    }

    public void SetBossHpBar(string title, float bossStartingHp)
    {
        bossTitle.text = title;
        bossMaxHp = bossStartingHp;
    }

    public void UpdateBossHpBar(float currHp)
    {
        bossHp.UpdateBar(currHp, bossMaxHp);
    }

    private void BossBehavior_OnBossDeath()
    {
        if(endLevelOnBossDeath)
            EndLevel();
        else
            mainFlowchart.SendFungusMessage("boss dead");
    }

    // Tells fungus that we have finished the level and are ready to move on to the next scene
    public void EndLevel()
    {
        OnLevelEnd?.Invoke();
        OnLevelComplete?.Invoke();
        mainFlowchart.SendFungusMessage("LevelComplete");

        // Save the highest level completed but ignore if we are in Endless Mode
        if (!OmniController.omniController.endlessMode)
        {
            if (PlayerPrefs.GetInt("highestLevelCompleted", 0) < levelIndex)
            {
                PlayerPrefs.SetInt("highestLevelCompleted", levelIndex);
            }
        }

        // Unsubscribe to events at the end of the level
        UnsubAllEvents();
    }

    // Tells fungus to load in the game summary because we have no lives left
    public void GameOver()
    {
        OnLevelEnd?.Invoke();
        mainFlowchart.SendFungusMessage("GameOver");

        // Unsubscribe to events at the end of the level
        UnsubAllEvents();
    }

    // Depreciating
    public void FindPlayer()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            print("Ohshit! GM cannot find player!");
            return;
        }
        playerController = player.GetComponent<PlayerController>();
    }

    // Does what it says
    public void SpawnPlayer()
    {
        // If available lives, spawn the player
        if (playerLives > 0)
        {
            player = Instantiate(playerObject, transform.position, transform.rotation) as GameObject;
            Instantiate(fx, transform.position, transform.rotation);
            GetComponent<AudioSource>().Play();
            playerController = player.GetComponent<PlayerController>();
            // If we are spawning in for the first time, do not subtract a life
            if (!initialSpawn1)
                playerLives--;
            else
                initialSpawn1 = false;
        }
        else
        {
            // If both player 1 and player 2 are dead with no lives, end the game
            if( player == null && player2 == null)
                GameOver();
        }
    }
    // Same as above but for player 2 (can probably combine these two)
    public void SpawnPlayer2()
    {
        // If available lives, spawn the player
        if (playerLives > 0)
        {
            player2 = Instantiate(playerObject2, transform.position - (Vector3.up * 3f), transform.rotation) as GameObject;
            Instantiate(fx, transform.position, transform.rotation);
            GetComponent<AudioSource>().Play();
            playerController2 = player2.GetComponent<PlayerController>();
            playerController2.isPlayer2 = true;
            // If we are spawning in for the first time, do not subtract a life
            if (!initialSpawn2)
                playerLives--;
            else
                initialSpawn2 = false;
        }
        else
        {
            // If both player 1 and player 2 are dead with no lives, end the game
            if (player == null && player2 == null)
                GameOver();
        }
    }

    // Invoked on a player's death
    private void PlayerController_OnPlayerDeath(PlayerController pc)
    {
        hasDied = true;

        // If no lives are left and no players are alive, it is game over.
        if (playerLives <= 0)
        {
            playerLives = 0;
            if ((pc.isPlayer2 && player == null) || player2 == null)
                GameOver();    // Call fungus flowchart to end game when out of lives
        }
        else
            DeathText(pc.isPlayer2);    // Calls Fungus flowchart that will display a death flavor text then respawn player

        AddRawScore(-OmniController.omniController.deathPenalty);   // Lose score from dying
        StartCoroutine(CoolTimeSlowFX());   // Briefly slow down time when player dies
        ResetMultiplier();  // Set score multiplier to 0
    }

    // Tells fungus flowchart to say a death flavor text when player dies
    public void DeathText(bool p2)
    {
        if (p2)
        {
            mainFlowchart.SendFungusMessage("death p2");
            return;
        }
        mainFlowchart.SendFungusMessage("death");
    }

    // Returns true if the player has died at least once this level
    public bool HasDied()
    {
        return hasDied;
    }

    // Idk wtf this is
    public void SetPlayerShipTo(GameObject playerShip)
    {
        playerObject = playerShip;
        playerObject2 = null;
    }
    // Optional two player method
    public void SetPlayerShipTo(GameObject playerShip, GameObject player2Ship)
    {
        playerObject = playerShip;
        playerObject2 = player2Ship;
    }

    // Adds score accoutning for multiplier
    public void AddScore(int sc)
    {
        chainedKills++;
        score += (int)(sc * scoreMultiplier * OmniController.omniController.additionalScoreMultiplier);
        endMultiplierTime = Time.time + multiplierTimer;
        if (chainedKills >= scoreMultiplier * 5 && scoreMultiplier < 10)
        {
            scoreMultiplier++;
            if (scoreMultiplier == 10)
                scoreMultiplierAnim.SetBool("MaxMultiplier", true);
            else
                scoreMultiplierAnim.SetBool("MaxMultiplier", false);
            scoreMultiplierAnim.SetTrigger("NewMultiplier");
        }
        score = Mathf.Clamp(score, 0, int.MaxValue);
    }

    // Applies score with no regard to multiplier
    public void AddRawScore(int sc)
    {
        score += (int)(sc * OmniController.omniController.additionalScoreMultiplier);
        score = Mathf.Clamp(score, 0, int.MaxValue);
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1;
        chainedKills = 0;
        scoreMultiplierAnim.SetBool("MaxMultiplier", false);
    }

    // Calculates and displays score when the level ends
    public int FinalScore()
    {
        int finalScore = score;
        int ammoScr = CalcAmmoScore();
        int hpScr = CalcHpScore();
        baseScore.text = "Base Score: " + finalScore;
        ammoScore.text = "Ammo Score: +" + ammoScr;
        hullScore.text = "Hull Score: +" + hpScr;

        int total = finalScore + ammoScr + hpScr;

        totalScore.text = "Total Score: " + total;

        if(OmniController.omniController != null)
            OmniController.omniController.totalScore += total;
        return total;
    }

    public int CalcAmmoScore()
    {
        // Prevent possible error if a player is dead when score is calculated
        float player1PercentAmmo;
        float player2PercentAmmo;

        if (player != null)
            player1PercentAmmo = (float)playerController.currentMissileCount / (float)playerController.maxMissile;
        else
            player1PercentAmmo = 0f;

        if (player2 != null)
            player2PercentAmmo = (float)playerController2.currentMissileCount / (float)playerController2.maxMissile;
        else
            player2PercentAmmo = 0f;

        if (!twoPlayerMode)
            return (int)(player1PercentAmmo * baseAmmoScore);
        else
            return (int)((player1PercentAmmo + player2PercentAmmo / 2f) * baseAmmoScore);
    }

    public int CalcHpScore()
    {
        // Prevent possible error if a player is dead when score is calculated
        float player1PercentHp;
        float player2PercentHp;

        if (player != null)
            player1PercentHp = playerController.hp / playerController.maxHp;
        else
            player1PercentHp = 0f;

        if (player2 != null)
            player2PercentHp = playerController2.hp / playerController2.maxHp;
        else
            player2PercentHp = 0f;

        if (!twoPlayerMode)
            return (int)(player1PercentHp * baseHpScore);
        else
            return (int)((player1PercentHp + player2PercentHp) / 2f * baseHpScore);
    }

    public void UnlockShipInLevel(ShipsEnum.ShipID id)
    {
        OmniController.omniController.UnlockShip((int)id);
    }

    public void SetLevelBounds(float left, float right, float up, float down)
    {
        upperBounds = up;
        lowerBounds = down;
        rightBounds = right;
        leftBounds = left;
    }

    public void ResetLevelBounds()
    {
        upperBounds = LEVEL_UPPER_BOUND;
        lowerBounds = LEVEL_LOWER_BOUND;
        rightBounds = LEVEL_RIGHT_BOUND;
        leftBounds = LEVEL_LEFT_BOUND;
    }

    public void SetGameState(bool start)
    {
        gameStart = start;
    }

    public void SetTimeScale(float x)
    {
        Time.timeScale = x;
    }

    // Pauses and unpauses game and displays or hides pause menu
    public void PauseGame(bool paused)
    {
        gamePaused = paused;
        pauseMenu.SetActive(paused);
        if(paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = OmniController.omniController.globalTimeScale;
    }

    // Method to tell Omnicontroller that we have won the game. Used by fungus on the last level.
    public void CompleteGame()
    {
        OmniController.omniController.completedGame = true;
    }

    // Unsubscribes all listeners in this class. Must call this before every level transition or wierd things happen
    private void UnsubAllEvents()
    {
        GameStarter.OnGameStart -= GameStarter_OnGameStart;
        PlayerController.OnPlayerDeath -= PlayerController_OnPlayerDeath;
        BossBehavior.OnBossDeath -= BossBehavior_OnBossDeath;
    }

    private IEnumerator CoolTimeSlowFX()
    {
        float currTimeScale = .3f;

        SetTimeScale(currTimeScale);

        yield return new WaitForSecondsRealtime(.8f);

        while (currTimeScale < OmniController.omniController.globalTimeScale)
        {
            if (!gamePaused)
            {
                currTimeScale += .1f;
                SetTimeScale(currTimeScale);
            }
            yield return new WaitForSecondsRealtime(.2f);
        }

        SetTimeScale(OmniController.omniController.globalTimeScale);
    }

    public void ExitToMainMenu()
    {
        OnLevelEnd?.Invoke();
        OnExitToMainMenu?.Invoke();

        // Unsubscribe to events at the end of the level
        UnsubAllEvents();

        SceneLoader.Instance.LoadScene("MainMenu");
    }
}
