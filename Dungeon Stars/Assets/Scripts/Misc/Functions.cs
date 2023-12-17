using UnityEngine;

public static class Functions
{
    /// <summary>
    /// Finds the closest GameObject of the specified tag to the specified transform
    /// </summary>
    /// <param name="tag">Tag of the GameObject to search for</param>
    /// <param name="transform">Transform of the GameObject to compare to</param>
    /// <returns>A GameObject of the closest thing with the specified tag</returns>
    public static GameObject FindClosestByTag(string tag, Transform transform)
    {
        // For targeting the player, we dont need to use FindGameObjectByTag()
        if (tag == Tags.Player)
        {
            GM gm = GM.GameController;

            // If player 2 does not exist but player 1 does, target player 1
            if (gm.player2 == null && gm.player != null)
            {
                return gm.player;
            }
            // If player 1 does not exist but player 2 does, target player 2
            else if (gm.player == null && gm.player2 != null)
            {
                return gm.player2;
            }
            // If both exist, compare the distance between us and both players and return closer one
            else if (gm.player != null && gm.player2 != null)
            {
                if (Vector3.Distance(transform.position, gm.player.transform.position) <=
                Vector3.Distance(transform.position, gm.player2.transform.position))
                    return gm.player;
                else
                    return gm.player2;
            }
            // If none exist, return null
            else
            {
                return null;
            }
        }

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if ((bool)go.GetComponent<ObstacleBehavior>()?.awake)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
