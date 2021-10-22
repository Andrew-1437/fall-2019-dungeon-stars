using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipBehavior : LargeEnemyBehavior
{
    short startingTurrets;

    // References to game objects that are visual fx of the damage done to the ship
    [Tooltip("References to game objects that are visual fx of the damage done to the ship." +
        "\nSort in order of [Most Damage ---> Less Damage]" +
        "\nDo not consider no damage as a state. Remember they are additive.")]
    public GameObject[] damageStates;


    // Start is called before the first frame update
    new void Start()
    {
        startingTurrets = turrets;
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        UpdateDamageState();

        base.Update();
    }

    // Enables the game object that is a visual fx of the damage (fire/smoke/etc) depending on how much damage we have sustained
    void UpdateDamageState()
    {
        // Check each of the damage states to see if we have met the threshold to enable the damage fx
        for(int i = 0; i < damageStates.Length; i++)
        {
            // Scales in proportion with how many damage states we have.
            // For example, if we have 1 damage state, it activates at half hp (or turrets in this case)
            if(turrets < startingTurrets * ((i + 1) / (float)(damageStates.Length + 1)))
            {
                damageStates[i].SetActive(true);
            }
        }
    }
}
