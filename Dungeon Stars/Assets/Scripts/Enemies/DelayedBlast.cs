using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedBlast : MonoBehaviour
{
    public GameObject Explosion;
    public float Delay;
    public Transform Indicator;

    private float startTime;
    private float endTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        endTime = startTime + Delay;
    }

    private void FixedUpdate()
    {
        Indicator.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime)/Delay);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(Instantiate(Explosion, transform.position, transform.rotation), 10f);
            Destroy(gameObject, .1f);
        }
    }
}
