using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour {

    public Text health;
    public Text shields;

    public GameObject playerObject;
    public GameObject fx;
    private GameObject player;
    public int playerLives;
    private PlayerController playerController;
    private GameObject enemies;
    private GameObject hud;
    private GameObject boss;
    private GameObject bossWarnUI;

    public bool gameStart;

    public bool allowBoss;

    public SimpleHealthBar hp;
    public SimpleHealthBar shield;

    public Fungus.Flowchart mainFlowchart;



    private void Start()
    {
        /*
        player = GameObject.FindWithTag("Player");
        if(player == null)
        {
            print("Ohshit. Player not found!");
        } */
        enemies = GameObject.FindWithTag("AllEnemies");
        if (enemies == null)
        {
            print("Ohshit. Enemies not found!");
        }
        hud = GameObject.FindWithTag("GUI");
        if (hud == null)
        {
            print("Ohshit. GUI not found!");
        }
        if (allowBoss) //If Level has a boss
        {
            boss = GameObject.FindWithTag("Boss");
            if (boss == null)
            {
                print("Ohshit. Boss not found!");
            }
            bossWarnUI = GameObject.FindWithTag("BossWarn");
            if (bossWarnUI == null)
            {
                print("Ohshit. Boss Warning not found!");
            }
        }

        GameObject selection = GameObject.FindWithTag("Selections");
        playerObject = selection.GetComponent<MaintainSelection>().selectedShip;

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
            hud.SetActive(!hud.activeSelf);
            if (hud.activeSelf)
            {
                print("HUD enabled");
            }
            else
            {
                print("HUD disabled");
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

        //Summons Boss
        if(Input.GetKeyDown("b") && allowBoss)
        {
            AwakenBoss();
        }
        //GUI Updates
        if (player != null)
        {
            health.text = "Health: " + Mathf.FloorToInt(playerController.hp);
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
                shields.text = "Shields: " + Mathf.FloorToInt(playerController.shield);
                shields.color = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f);
            }
            else
            {
                shields.text = "Shields: =OFFLINE=";
                shields.color = new Color(0.9f, 0.0f, 0.0f);
            }

            hp.UpdateBar(playerController.hp, playerController.maxHp);
            shield.UpdateBar(playerController.shield, playerController.maxShield);
        }
        else
        {
            health.text = "Health: =NULL=";
            shields.text = "Shields: =NULL=";
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
