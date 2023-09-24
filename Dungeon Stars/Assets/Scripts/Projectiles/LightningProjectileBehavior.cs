using UnityEngine;


public class LightningProjectileBehavior : ProjectileBehavior
{
    public float MaxAnglePerFrame;

    protected void FixedUpdate()
    {
        if (MaxAnglePerFrame > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f,
                transform.rotation.eulerAngles.z + Random.Range(-MaxAnglePerFrame, MaxAnglePerFrame));
        }

        base.FixedUpdate();
    }
    
}
