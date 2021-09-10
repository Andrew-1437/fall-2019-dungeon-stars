using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashUI : MonoBehaviour {

    public bool awake;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void wake()
    {
        awake = true;
    }

    public void Flash()
    {
        anim.SetTrigger("Start");
    }
}
