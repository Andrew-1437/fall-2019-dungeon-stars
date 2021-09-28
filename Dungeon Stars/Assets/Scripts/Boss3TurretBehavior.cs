using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3TurretBehavior : TurretBehavior
{
    public GameObject extraProjectile;

    public bool doSpinAttack;
    public float spinSpeed;

    private float angle = 0;

    private new void Start()
    {
        base.Start();
        
        OnBurstEnd += Boss3TurretBehavior_OnBurstEnd;
        GM.OnLevelEnd += GM_OnLevelEnd;
    }

    // Update is called once per frame
    private new void Update()
    {
        if (!doSpinAttack)
            base.Update();
        else
            SpinAttack();
    }

    private void Boss3TurretBehavior_OnBurstEnd()
    {
        StartCoroutine(FireExtraProjectile());
    }

    private IEnumerator FireExtraProjectile()
    {
        yield return new WaitForSeconds(1f);

        Destroy(
            Instantiate(extraProjectile, hardpoint.position, hardpoint.rotation),
            5f);
    }

    private void SpinAttack()
    {
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        angle += spinSpeed * Time.deltaTime;

        if (Time.time > nextFire)
        {
            Fire();
        }
    }

    private void OnDestroy()
    {
        OnBurstEnd -= Boss3TurretBehavior_OnBurstEnd;
        GM.OnLevelEnd -= GM_OnLevelEnd;
    }

    private void GM_OnLevelEnd()
    {
        OnBurstEnd -= Boss3TurretBehavior_OnBurstEnd;
        GM.OnLevelEnd -= GM_OnLevelEnd;
    }
}
