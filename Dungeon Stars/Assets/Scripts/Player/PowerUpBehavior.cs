using System.Collections;
using TMPro;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour {

    public enum PowerUps { None, LevelUp, Repair, HpRepair, SpeedUp, FireUp, Ammo, BoltDrone };

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
        if (GM.gameController.gameStart)
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
    /// Will add to the player's score, play a sound, and delete this power up
    /// </summary>
    public void OnCollected()
    {
        collectAudio.Play();
        GM.gameController.AddScore(score);
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
            Random.Range(GM.gameController.leftBounds, GM.gameController.rightBounds),
            GM.gameController.upperBounds);
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
