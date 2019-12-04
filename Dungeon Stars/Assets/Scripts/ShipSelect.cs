using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShipSelect : MonoBehaviour
{
    public GameObject[] ships;
    private GameObject currentShip;
    private PlayerController ship;
    private int index = 0;

    public Transform spawn;

    public SceneLoader sceneLoader;
    public MaintainSelection selection;
    public string nextSceneName;

    [Header("UI Elements")]
    public GameObject canvas;
    public TextMeshProUGUI shipName;
    public TextMeshProUGUI health;
    public TextMeshProUGUI shield;
    public TextMeshProUGUI shieldRecharge;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI primary;
    public TextMeshProUGUI secondary;
    public TextMeshProUGUI tertiary;
    public TextMeshProUGUI description;

    private AudioSource sound;
    private bool p2Picking = false;

    // Start is called before the first frame update
    void Start()
    {
        SelectNextShip(0);
        sound = GetComponent<AudioSource>();
        if(OmniController.omniController.enableAllShips)
        {
            ships = OmniController.omniController.allShips;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shipName.text = ship.shipName;
        health.text = "Hull: " + ship.maxHp;
        shield.text = "Shield: " + ship.maxShield;
        shieldRecharge.text = "Shield Recharge: " + (ship.shieldRecharge*30f) + "(" + ship.shieldRegenDelay + "s)";
        speed.text = "Speed: " + ship.speed;
        primary.text = "Primary Weapons: " + ship.primaryWeap;
        secondary.text = "Secondary Weapons: " + ship.secondaryWeap;
        tertiary.text = "Tertiary Weapons: " + ship.tertiaryWeap;
        description.text = ship.desc;

        if(Input.GetKeyDown(KeyCode.E))
        {
            SelectNextShip();
            sound.Play();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectNextShip(-1);
            sound.Play();
        }

        if (Input.GetKeyDown(KeyCode.Return) && OmniController.omniController.twoPlayerMode && p2Picking)
        {
            selection.selectedShip2 = ships[index];
            SceneLoader.DontDestroyOnLoad(selection.gameObject);
            //SceneManager.LoadScene(nextSceneName);
            canvas.SetActive(false);
            sceneLoader.LoadScene(nextSceneName);
        }

        if (Input.GetKeyDown(KeyCode.Return) && !p2Picking)
        {
            selection.selectedShip = ships[index];
            if (OmniController.omniController.twoPlayerMode)
            {
                p2Picking = true;
                print("Player 1 ship selected");
            }
            else
            {
                selection.selectedShip2 = null;
                SceneLoader.DontDestroyOnLoad(selection.gameObject);
                //SceneManager.LoadScene(nextSceneName);
                canvas.SetActive(false);
                sceneLoader.LoadScene(nextSceneName);
            }
        }

        

    }

    public void SelectNextShip(int select = 1)
    {
        index = (index + select);
        if (index < 0) index = ships.Length - 1;
        index = index % ships.Length;
        if (currentShip) { Destroy(currentShip); }

        currentShip = Instantiate(ships[index], spawn) as GameObject;
        ship = currentShip.GetComponent<PlayerController>();
    }
}
