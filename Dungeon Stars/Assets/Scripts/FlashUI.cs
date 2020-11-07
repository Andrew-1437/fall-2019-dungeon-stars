using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashUI : MonoBehaviour {

    public bool awake;
    private float alpha;
    public float deltaAlpha;
    public short flashes;
    private short currFlashes;
    private CanvasRenderer[] elements;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        /*
        alpha = 0.0f;
        currFlashes = 0;
        elements = gameObject.GetComponentsInChildren<CanvasRenderer>();
        elements[0].SetColor(new Color(1.0f, 1.0f, 1.0f, alpha));
        elements[1].SetColor(new Color(1.0f, 1.0f, 1.0f, alpha));*/
    }

    private void Update()
    {
        /*
        if(awake)
        {
            if((alpha <= 0.0f && deltaAlpha < 0.0f && currFlashes < flashes) || (alpha >= 1.0f && deltaAlpha > 0.0f))
            {
                deltaAlpha = (-1.0f) * deltaAlpha;
            }
            alpha += deltaAlpha;
            if (alpha >= 1.0f)
            {
                currFlashes++;
            }
        }
        elements[0].SetColor(new Color(1.0f, 1.0f, 1.0f, alpha));
        elements[1].SetColor(new Color(1.0f, 1.0f, 1.0f, alpha)); */
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
