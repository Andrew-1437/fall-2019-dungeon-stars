using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyIndicator : MonoBehaviour
{
    private float MaxVert;
    private float MaxHoriz;
    private float UpperBound;

    const float MinScale = 0.6f;
    const float MaxScale = 1.8f;

    public Transform Indicator;
    public Transform Arrow;

    private Transform TargetTransform;
    private Rigidbody2D TargetRb;
    private StdEnemyBehavior TargetBehavior;
    private float Offset;


    public void InitIndicator(GameObject target)
    {
        TargetTransform = target.transform;
        TargetRb = target.GetComponent<Rigidbody2D>();
        TargetBehavior = target.GetComponent<StdEnemyBehavior>();
        CapsuleCollider2D coll = target.GetComponent<CapsuleCollider2D>();
        Offset = coll ? coll.size.y / 2f : 0;

        transform.rotation = Quaternion.identity;

        GetCurrentScreenBorders();
    }

    private void Update()
    {
        if (TargetTransform == null) { return; }

        if (!ShouldShow()) 
        {
            Indicator.gameObject.SetActive(false);

            return; 
        }

        Indicator.gameObject.SetActive(true);

        // Indicator should stay at the border of the screen
        Vector3 pos = TargetTransform.position;
        float newX = pos.x > MaxHoriz ? MaxHoriz : 
            pos.x < -MaxHoriz ? -MaxHoriz : 
            pos.x;
        float newY = pos.y > MaxVert ? MaxVert :
            pos.y < -MaxVert ? -MaxVert :
            pos.y;
        transform.position = new Vector3(newX, newY);

        // Indicator should increase in size the closer the target
        float distance = Vector3.Distance(transform.position, TargetTransform.position);
        float newScale = Mathf.Lerp(MaxScale, MinScale, distance / 6f);
        Indicator.localScale = new Vector3(newScale, newScale, 1);

        // Arrow should point towards the target
        Arrow.transform.up = TargetTransform.position - transform.position;
    }

    private void GetCurrentScreenBorders()
    {
        Vector3 topRight = new Vector3(Screen.width - 50, Screen.height - 50);

        Vector3 bounds = Camera.main.ScreenToWorldPoint(topRight);

        MaxHoriz = bounds.x;
        MaxVert = bounds.y;
        UpperBound = MaxVert + 3.5f;
    }

    private bool ShouldShow()
    {
        Vector3 pos = TargetTransform.position;

        if (TargetBehavior != null && TargetBehavior.awake)
        {
            return (pos.x < -MaxHoriz - 1f || pos.x > MaxHoriz + 1f) || (pos.y < -MaxVert - 1f || pos.y > MaxVert + 1f);
        }

        float currY = pos.y - Offset;

        return (currY + (TargetRb.velocity.y * 3)) < UpperBound;
    }
}
