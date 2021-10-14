using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShipSelect : MonoBehaviour
{
    GameObject[] ships;
    public GameObject[] previewingShips;
    private GameObject currentShip;
    private ShipPreview ship;
    private int index = 0;

    public Transform spawn;

    public SceneLoader sceneLoader;
    public MaintainSelection selection;

    [Header("UI Elements")]
    public GameObject canvas;
    public TextMeshProUGUI title;
    public TextMeshProUGUI shipName;
    public TextMeshProUGUI health;
    public TextMeshProUGUI shield;
    public TextMeshProUGUI shieldRecharge;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI primaryFR;
    public TextMeshProUGUI secondaryFR;
    public TextMeshProUGUI primary;
    public TextMeshProUGUI secondary;
    public TextMeshProUGUI tertiary;
    public TextMeshProUGUI description;

    public AudioSource selectSound;
    private AudioSource sound;
    private bool p2Picking = false;

    // Start is called before the first frame update
    void Start()
    {
        OmniController.omniController.LoadUnlockedShips();
        ships = OmniController.omniController.allShips;
        SelectNextShip(0);
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ship.unlocked)
        {
            shipName.text = ship.shipName;
            health.text = "Hull: " + ship.maxHP;
            shield.text = "Shield: " + ship.maxShield;
            shieldRecharge.text = "Shield Recharge: " + (ship.shieldRecharge * 30f);
            speed.text = "Speed: " + ship.speed;
            primary.text = "Primary Weapons: " + ship.primaryWeap;
            secondary.text = "Secondary Weapons: " + ship.secondaryWeap;
            primaryFR.text = "Primary Fire Rate: " + (1f / ship.primaryFireRate) + " / sec ";
            secondaryFR.text = "Secondary Fire Rate: " + (1f / ship.secondaryFireRate) + " / sec ";
            tertiary.text = "Tertiary Weapons: " + ship.tertiaryWeap;
            description.text = ship.shipDesc;
        }
        else
        {
            shipName.text = "???";
            health.text = "Hull: ???";
            shield.text = "Shield: ???";
            shieldRecharge.text = "Shield Recharge: ???";
            speed.text = "Speed: ???";
            primary.text = "Primary Weapons: ???";
            secondary.text = "Secondary Weapons: ???";
            primaryFR.text = "Primary Fire Rate: ???";
            secondaryFR.text = "Secondary Fire Rate: ???";
            tertiary.text = "Tertiary Weapons: ???";
            description.text = "To Unlock: " + ship.howToUnlock;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectNextShip();
            sound.Play();
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectNextShip(-1);
            sound.Play();
        }

        if (Input.GetKeyDown(KeyCode.Return) && ship.unlocked && OmniController.omniController.twoPlayerMode && p2Picking)
        {
            ship.LeaveScreen();
            selectSound.Play();
            OmniController.omniController.selectedShip2 = ships[index];
            SceneLoader.DontDestroyOnLoad(selection.gameObject);
            canvas.SetActive(false);
            sceneLoader.LoadScene(OmniController.omniController.loadIntoLevel);
        }

        if (Input.GetKeyDown(KeyCode.Return) && ship.unlocked && !p2Picking)
        {
            selectSound.Play();
            OmniController.omniController.selectedShip = ships[index];
            if (OmniController.omniController.twoPlayerMode)
            {
                p2Picking = true;
                //print("Player 1 ship selected");
                SelectShipIndex(0);
            }
            else
            {
                ship.LeaveScreen();
                OmniController.omniController.selectedShip2 = null;
                SceneLoader.DontDestroyOnLoad(selection.gameObject);
                //SceneManager.LoadScene(nextSceneName);
                canvas.SetActive(false);
                sceneLoader.LoadScene(OmniController.omniController.loadIntoLevel);
            }
        }

        if (OmniController.omniController.enableDebug && Input.GetKeyDown(KeyCode.Space))
        {
            OmniController.omniController.UnlockShip((int)ship.id);
        }

        if(!OmniController.omniController.twoPlayerMode)
        {
            title.text = "Select your ship";
        }
        else
        {
            if(!p2Picking)
            {
                title.text = "Player 1, select your ship";
            }
            else
            {
                title.text = "Player 2, select your ship";
            }
        }
        

    }

    public void SelectNextShip(int select = 1)
    {
        index = (index + select);
        if (index < 0) index = ships.Length - 1;
        index = index % ships.Length;
        if (currentShip) { ship.LeaveScreen(); }

        currentShip = Instantiate(previewingShips[index], spawn) as GameObject;
        ship = currentShip.GetComponent<ShipPreview>();
        ship.unlocked = OmniController.omniController.unlockedShips[index];
    }

    public void SelectShipIndex(int ind)
    {
        index = ind;
        if (index < 0) index = ships.Length - 1;
        index = index % ships.Length;
        if (currentShip) { ship.LeaveScreen(); }

        currentShip = Instantiate(previewingShips[index], spawn) as GameObject;
        ship = currentShip.GetComponent<ShipPreview>();
        ship.unlocked = OmniController.omniController.unlockedShips[index];
    }
}
