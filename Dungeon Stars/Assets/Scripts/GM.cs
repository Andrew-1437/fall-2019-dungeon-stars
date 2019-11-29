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
            lives.text = "Lives: " + playerLives;
            scores.text = "Score: "+score.ToString();

            hp.UpdateBar(playerController.hp, playerController.maxHp);
            shield.UpdateBar(playerController.shield, playerController.maxShield);
            heatBar.SetActive(playerController.enableHeat);
            if(playerController.enableHeat)
            {
                heat.UpdateBar(playerController.heat, 100f);
                Color heatColor = new Color(1f, 230f / 255f, 0f);
                if(playerController.overheating)
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
            playerLives--;
        }
        else
        {
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
            playerLives--;
        }
        else
        {
            mainFlowchart.SendFungusMessage("GameOver");
            //print("no lives");
        }
    }

    public void DeathText()
    {
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
        return (int)(((float)playerController.currentMissileCount / (float)playerController.maxMissile) * baseAmmoScore);
    }

    public int CalcHpScore()
    {
        return (int)((playerController.hp / playerController.maxHp) * baseHpScore);
    }
}
