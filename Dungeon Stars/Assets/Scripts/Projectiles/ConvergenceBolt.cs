using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvergenceBolt : ProjectileBehavior
{
    public float initForce;
    public float convergeForce;
    [Tooltip("Each frame, multiple the convergence force by this amount")]
    public float convergeStrength;
    public string targetTag;
    
    Rigidbody2D rb;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * initForce, ForceMode2D.Impulse);
        GameObject t = Functions.FindClosestByTag(targetTag, transform);
        if (t)
            target = t.transform;
    }

    private void FixedUpdate()
    {
        Vector3 targetDir = transform.up;
        if (target)
        {
            targetDir = target.position - transform.position;
        }
        else
        {
            GameObject t = Functions.FindClosestByTag(targetTag,transform);
            if (t)
                target = t.transform;
        }
        rb.AddForce(targetDir.normalized * convergeForce);
        convergeForce *= convergeStrength;
    }
}
