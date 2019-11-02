using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {

    [Header("References")]
    public GameObject playerObject;
    public GameObject fx;
    private GameObject player;
    public int playerLives;
    private PlayerController playerController;
    public GameObject enemies;
    public GameObject ui;
    public GameObject boss;
    public GameObject bossWarnUI;
    private BossBehavior bossStats;
    private float bossMaxHp;

    public bool gameStart;

    public bool allowBoss;

    [Header("UI")]
    public Text health;
    public Text shields;
    public Text lives;
    public SimpleHealthBar hp;
    public SimpleHealthBar shield;
    public SimpleHealthBar bossHp;
    public Text missileCount;

    [Header("Flowchart")]
    public Fungus.Flowchart mainFlowchart;



    private void Start()
    {
        GameObject selection = GameObject.FindWithTag("Selections");
        if (selection)
        {
            playerObject = selection.GetComponent<MaintainSelection>().selectedShip;
        }
        if (boss)
        {
            bossStats = boss.GetComponent<BossBehavior>();
            bossMaxHp = bossStats.hp;
        }
    }

    private void Update()
    {
        //Debug*************
        //Toggle Enemies
        if(Input.GetKeyDown("1"))
        {
            enemies.SetActive(!enemies.activeSelf);
            if(enemies.activeSelf)
            {
                print("Enemies enabled");
            }
            else
            {
                print("Enemies disabled");
            }
        }
        //Toggle GUI Elements
        if(Input.GetKeyDown("2"))
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
        if(Input.GetKeyDown("0"))
        {
            print("Respawning Player...");
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.GetComponent<PlayerController>().Die();
            }
            SpawnPlayer();
            
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
        //Quick exit to menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        //Summons Boss
        if(Input.GetKeyDown("b") && allowBoss)
        {
            AwakenBoss();
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
                shields.color = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f);
            }
            else
            {
                shields.text = "Shields: =OFFLINE=";
                shields.color = new Color(0.9f, 0.0f, 0.0f);
            }

            missileCount.text = "Tertiary Ammo: " + playerController.currentMissileCount + "/" + playerController.maxMissile;
            lives.text = "Lives: " + playerLives;

            hp.UpdateBar(playerController.hp, playerController.maxHp);
            shield.UpdateBar(playerController.shield, playerController.maxShield);
            if (boss)
            {
                bossHp.UpdateBar(bossStats.hp, bossMaxHp);
            }
        }
        else
        {
            health.text = "Health: ";
            shields.text = "Shields: ";
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
            Instantiate(playerObject, transform.position, transform.rotation);
            Instantiate(fx, transform.position, transform.rotation);
            GetComponent<AudioSource>().Play();
            FindPlayer();
            playerLives--;
        }
        else
        {
            print("no lives");
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
    }
}
