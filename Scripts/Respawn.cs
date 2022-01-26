using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private int randomRespawnArea;
    private float randomXSpawnBoundry, randomYSpawnBoundry;
    private Vector3 spawnPos;

    public void SpawnChaser()
    {
        randomRespawnArea = UnityEngine.Random.Range(0, 4);

        switch (randomRespawnArea)
        {
            case 0:
                randomXSpawnBoundry = UnityEngine.Random.Range(-32.5f, -18f);
                randomYSpawnBoundry = UnityEngine.Random.Range(-4f, -8f); //spawns in random spot based off obe of these 4 cases
                break;
            case 1:
                randomXSpawnBoundry = UnityEngine.Random.Range(-14f, -25f);
                randomYSpawnBoundry = UnityEngine.Random.Range(-15f, -11f);
                break;

            case 2:
                randomXSpawnBoundry = UnityEngine.Random.Range(17f, 5f);
                randomYSpawnBoundry = UnityEngine.Random.Range(-24f, -24f);
                break;
            case 3:
                randomXSpawnBoundry = UnityEngine.Random.Range(5.5f, 19f);
                randomYSpawnBoundry = UnityEngine.Random.Range(-18f, -18f);
                break;

        }

        spawnPos = new Vector3(randomXSpawnBoundry, randomYSpawnBoundry, 0f);
        transform.position = spawnPos;
    }
}
