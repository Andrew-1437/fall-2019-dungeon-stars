using System.Collections;
using TMPro;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour {

    public enum PowerUps 
    { 
        None, 
        LevelUp, 
        Repair, 
        HpRepair, 
        SpeedUp, 
        FireUp, 
        Ammo, 
        BoltDrone, 
        Cleanse 
    };

    public PowerUps type;

    public float speed;
    public float duration;
    public int score;

    [Tooltip("GameObject that is summoned when player collects")]
    public GameObject summon;
    [Tooltip("Text that pops up when player collects power up")]
    public GameObject popupText;   

    private bool awake;

    [Tooltip("Used to identify power up in scene editor")]
    public GameObject marker;  // Used to identify in scene in edit mode
    Rigidbody2D rb;
    public AudioSource collectAudio;

    private void Start()
    {
        awake = false;

        rb = GetComponent<Rigidbody2D>();

        marker.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (GM.GameController.gameStart)
        {
            if (awake)
            {
               rb.velocity = (Vector3.down * speed) + Vector3.down;
            }
            else
            {
                rb.velocity = Vector3.down;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }

    /// <summary>
    /// Applies the power up's effect to the player
    /// </summary>
    /// <param name="player">Player controller that triggered the power up</param>
    public void ApplyPowerUp(PlayerController player)
    {
        switch (type)
        {
            // Immediately restore half the shield
            case PowerUps.Repair:
                player.shield = Mathf.Min(player.maxShield, player.shield + player.maxShield * 0.5f);
                player.shieldDown = false;
                player.shieldSprite.SetTrigger("Restored");
                player.hex.CleanseHex();
                break;

            // Immediately restore 75% of missing hp and shield
            case PowerUps.HpRepair:
                player.hp = Mathf.Min(player.maxHp, player.hp + (player.maxHp - player.hp) * 0.75f);
                player.shield = Mathf.Min(player.maxShield, player.shield + (player.maxShield - player.shield) * 0.75f);
                player.shieldDown = false;
                player.shieldSprite.SetTrigger("Restored");
                player.hex.CleanseHex();
                break;

            // Increases fire rate and reduces heat gen
            case PowerUps.FireUp:
                player.fireRateMod = 0.75f;
                player.heatGenMod = 0.2f;
                player.fireRateEnd = Time.time + duration * OmniController.omniController.powerUpDurationScale;
                player.fireRateFX.Play();
                break;

            // Increases speed & prevents Hex buildup during that time
            case PowerUps.SpeedUp:
                player.speedMod = 1.25f;
                player.speedEnd = Time.time + duration * OmniController.omniController.powerUpDurationScale;
                player.speedFX.Play();
                player.hex.CleanseHex(duration);
                break;

            // Increases power level by 1
            case PowerUps.LevelUp:
                player.LevelUp();
                player.hex.CleanseHex();
                break;

            // Resets ammo to max
            case PowerUps.Ammo:
                player.primary.ReplenishAmmo();
                player.secondary.ReplenishAmmo();
                player.explosive.ReplenishAmmo();
                break;

            // Summons a bolt-shooting drone to assist
            case PowerUps.BoltDrone:
                Summon(player.isPlayer2);
                break;

            // Removes Hex debuff
            case PowerUps.Cleanse:
                player.hex.CleanseHex();
                break;

            // Shouldn't be reachable but just in case
            default:
                break;
        }
    }

    /// <summary>
    /// Instantiates a drone that will follow the player around
    /// </summary>
    /// <param name="byPlayerTwo">Set to true if summoned by player two</param>
    public void Summon(bool byPlayerTwo)
    {
        GameObject summonedObject = Instantiate(summon, transform.position, transform.rotation);
        DroneBehavior summonedDrone = summonedObject.GetComponent<DroneBehavior>();
        summonedDrone.lifetime = duration * OmniController.omniController.powerUpDurationScale;
        summonedDrone.followPlayer2 = byPlayerTwo;
    }

    /// <summary>
    /// When the power up is collected by a player
    /// will apply the power up's effects,
    /// add to the player's score, play a sound, and delete this power up
    /// </summary>
    public void OnCollected(PlayerController player)
    {
        ApplyPowerUp(player);

        collectAudio.Play();
        GM.GameController.AddScore(score);
        collectAudio.gameObject.transform.parent = null;
        Destroy(gameObject);
        Destroy(collectAudio.gameObject, 2f);
        if(popupText)
        {
            GameObject flashText = Instantiate(popupText,
                transform.position,
                Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f))
                );
            flashText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere, ForceMode2D.Impulse);
            Destroy(flashText, 1);
        }
    }

    /// <summary>
    /// Wrapper tag to send the power up to the top of the screen when dropped by a dying player
    /// </summary>
    public void GoToScreenTop()
    {
        StartCoroutine(LerpToScreenTop());
    }

    /// <summary>
    /// Moves the power up to the top of the screen over the course of a few seconds
    /// </summary>
    private IEnumerator LerpToScreenTop()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(
            Random.Range(GM.GameController.leftBounds, GM.GameController.rightBounds),
            GM.GameController.upperBounds);
        float startTime = Time.time;
        float duration = Random.Range(1.2f, 2.2f);

        float t = 0;

        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);

            t = (Time.time - startTime) / duration;

            yield return new WaitForFixedUpdate();
        }

    }
}
