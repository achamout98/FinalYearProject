using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> RockPrefabs;
    [SerializeField] private float MaxTime = 5f;

    private List<Transform> SpawnPoints = new List<Transform>();

    private float remainingTime;

    private void Awake () {
        for(int i = 0; i < transform.childCount; i++) {
            SpawnPoints.Add(transform.GetChild(i));
        }
    }

    private void Start () {
        remainingTime = MaxTime;
    }

    private void Update () {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0) {
            int index = Random.Range(0, SpawnPoints.Count);
            Vector3 location = SpawnPoints[index].transform.position;

            index = Random.Range(0, RockPrefabs.Count);
            GameObject r = RockPrefabs[index];
            GameObject.Instantiate(r, location, Quaternion.identity);
            remainingTime = MaxTime;
        }
    }
    //public Transform[] spawnPoints;

    //public GameObject rockPreFab;

    //public float timeToSpawn = 5f;

    //public float timeBetweenWaves = 10f;

    //// Update is called once per frame
    //void Update()
    //{
    //    if(Time.time >= timeToSpawn)
    //    {
    //        SpawnRocks();
    //        timeToSpawn = Time.time + timeBetweenWaves;
    //    }
    //}

    //void SpawnRocks()
    //{
    //    int randomIndex = Random.Range(0, spawnPoints.Length);

    //    for (int i = 0; i < spawnPoints.Length; i++)
    //    {
    //        if (randomIndex == i)
    //        {
    //            Instantiate(rockPreFab, spawnPoints[i].position, Quaternion.identity);
    //        }
    //    }
    //}
}
