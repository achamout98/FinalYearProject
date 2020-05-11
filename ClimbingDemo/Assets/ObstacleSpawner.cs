using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public GameObject rockPreFab;

    public float timeToSpawn = 5f;

    public float timeBetweenWaves = 10f;

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= timeToSpawn)
        {
            SpawnRocks();
            timeToSpawn = Time.time + timeBetweenWaves;
        }
    }

    void SpawnRocks()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (randomIndex == i)
            {
                Instantiate(rockPreFab, spawnPoints[i].position, Quaternion.identity);
            }
        }
    }
}
