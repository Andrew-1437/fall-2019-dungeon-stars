using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarner : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
            anim.SetBool("Enemy", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
            anim.SetBool("Enemy", false);
    }


}
