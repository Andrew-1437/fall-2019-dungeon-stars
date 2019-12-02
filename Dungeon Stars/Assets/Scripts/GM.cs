using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GM : MonoBehaviour {

    [HideInInspector]
    public static GM gameController;

    [Header("References")]
    public GameObject playerObject;
    public GameObject playerObject2;
    public GameObject fx;
    private GameObject player;
    private GameObject player2;
    private PlayerController playerController;
    private PlayerController playerController2;
    public GameObject enemies;
    public GameObject ui;
    public GameObject boss;
    public GameObject bossWarnUI;
    private BossBehavior bossStats;
    private float bossMaxHp;

    [Header("Score")]
    public int score;
    public int baseAmmoScore;  // Highest score bonus if have max ammo 
    public int baseHpScore;  // Highest score bonus if have max hp

    [Header("Level Management")]
    public bool gameStart;
    public bool allowBoss;
    public int playerLives;
    public bool twoPlayerMode;

    [Header("UI")]
    public GameObject soloUIElements;
    public TextMeshProUGUI health;
    public TextMeshProUGUI shields;
    public TextMeshProUGUI level;
    public TextMeshProUGUI lives;
    public SimpleHealthBar hp;
    public SimpleHealthBar shield;
    public SimpleHealthBar bossHp;
    public TextMeshProUGUI missileCount;
    public GameObject heatBar;
    public SimpleHealthBar heat;
    public TextMeshProUGUI scores;
    public TextMeshProUGUI baseScore;
    public TextMeshProUGUI ammoScore;
    public TextMeshProUGUI hullScore;
    public TextMeshProUGUI totalScore;
    public TMP_ColorGradient shieldOnColor;
    public TMP_ColorGradient shieldOffColor;

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

    [Header("Flowchart")]
    public Fungus.Flowchart mainFlowchart;



    private void Awake()
    {
        twoPlayerMode = OmniController.omniController.twoPlayerMode;

        GameObject selection = GameObject.FindWithTag("Selections");
        if (selection)
        {
            playerObject = selection.GetComponent<MaintainSelection>().selectedShip;
            if (twoPlayerMode)
                playerObject2 = selection.GetComponent<MaintainSelection>().selectedShip2;
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
    }

    private void Update()
    {
        //Quick exit to menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        //Debug*************
        if (OmniController.omniController.enableDebug)
        {
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

            //Summons Boss
            if (Input.GetKeyDown("b") && allowBoss)
            {
                AwakenBoss();
            }
        }


        //GUI Updates
        lives.text = "Lives: " + playerLives;
        scores.text = "Score: " + score.ToString();

        // ONE PLAYER ===================
        if (!twoPlayerMode)
        {
            if (player != null)
            {
                health.text = "Health: " + Mathf.FloorToInt(playerController.hp) + "/" + Mathf.FloorToInt(playerController.maxHp);
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

                level.text = "Power: " + (playerController.level + 1);

                missileCount.text = "Ammo: " + playerController.currentMissileCount + "/" + playerController.maxMissile;

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

                level1.text = "Power: " + (playerController.level + 1);

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
                level.text = "Power: 0";
            }
            if (player2 != null)
            {
                health2.text = "Health: " + Mathf.FloorToInt(playerController2.hp) + "/" + Mathf.FloorToInt(playerController2.maxHp);
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

                level2.text = "Power: " + (playerController2.level + 1);

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
        if (boss)
        {
            bossHp.UpdateBar(bossStats.hp, bossMaxHp);
        }

        if (score<0)
        {
            score = 0;
        }
    }

    public void AwakenBoss()
    {
        boss.GetComponent<BossBehavior>().awake = true;
        bossWarnUI.GetComponent<FlashUI>().awake = true;
    }

    //Scene specific events ***Obsolete
    public void CallEvent(string x)
    {
        SendMessage(x);
        if (x == "Boss" && allowBoss)
        {
            AwakenBoss();
        }
    }

    public void EndLevel()
    {
        mainFlowchart.SendFungusMessage("LevelComplete");
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

    public void SpawnPlayer()
    {
        if (playerLives > 0)
        {
            player = Instantiate(playerObject, transform.position, transform.rotation) as GameObject;
            Instantiate(fx, transform.position, transform.rotation);
            GetComponent<AudioSource>().Play();
            //FindPlayer();
            playerController = player.GetComponent<PlayerController>();
            //playerLives--;
        }
        else
        {
            // If both player 1 and player 2 are dead with no lives, end the game
            if( player == null && player2 == null)
                mainFlowchart.SendFungusMessage("GameOver");
            //print("no lives");
        }
    }
    public void SpawnPlayer2()
    {
        if (playerLives > 0)
        {
            player2 = Instantiate(playerObject2, transform.position - (Vector3.up * 3f), transform.rotation) as GameObject;
            Instantiate(fx, transform.position, transform.rotation);
            GetComponent<AudioSource>().Play();
            //FindPlayer();
            playerController2 = player2.GetComponent<PlayerController>();
            playerController2.isPlayer2 = true;
            //playerLives--;
        }
        else
        {
            // If both player 1 and player 2 are dead with no lives, end the game
            if (player == null && player2 == null)
                mainFlowchart.SendFungusMessage("GameOver");
            //print("no lives");
        }
    }

    public void DeathText(bool p2)
    {
        if (p2)
        {
            mainFlowchart.SendFungusMessage("death p2");
            return;
        }
        mainFlowchart.SendFungusMessage("death");
        //int index = Random.Range(0, deathTexts.Length);
        //deathTexts[index].SetActive(true);
    }

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
        //print(OmniController.omniController.totalScore);
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
            return (int)(((player1PercentHp + player2PercentHp) / 2f) * baseHpScore);
    }
}
