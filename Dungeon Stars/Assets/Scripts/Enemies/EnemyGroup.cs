using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public ObstacleBehavior[] EnemyGroupList;
    private int EnemiesCount;

    // Events
    public delegate void EnemyGroupDelegate();
    public event EnemyGroupDelegate OnGroupKilled;

    void Start()
    {
        EnemiesCount = EnemyGroupList.Length;
        foreach (ObstacleBehavior enemy in EnemyGroupList) 
        {
            enemy.OnObstacleDeath += OnEnemyDeath;  // Subscriber to each obstacle's death
        }
    }

    /// <summary>
    /// Each obstacle behavior that dies invokes this function, reducing the number of enemies left in the group
    /// Once all the enemies in the group have been killed, this invokes the OnGroupKilled event
    /// which can be used by other things to trigger other events/dialogue/etc
    /// </summary>
    public void OnEnemyDeath(ObstacleBehavior enemy)
    {
        EnemiesCount--;
        enemy.OnObstacleDeath -= OnEnemyDeath;  // Unsubscribe to the obstacle's death once it dies

        if (EnemiesCount <= 0 )
        {
            OnGroupKilled?.Invoke();
            Destroy(gameObject, 1);
        }
    }
}
