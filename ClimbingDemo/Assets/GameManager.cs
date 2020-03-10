using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SpawnPoints;

    [SerializeField]
    private List<GameObject> prefabs;

    private List<GameObject> children;
    // Start is called before the first frame update
    void Start()
    {
        //int num = SpawnPoints.transform.childCount;

        //for(int i = 0; i < num; i++) {
        //    children[i] = SpawnPoints.transform.GetChild(i).gameObject;
        //}
        StartCoroutine("SpawnRocks");
    }

    private IEnumerator SpawnRocks () {
        //foreach(GameObject child in children) {
        //    Vector3 pos = child.transform.position;
        //    int index = Random.Range(0, prefabs.Count-1);
        //    Instantiate(prefabs[index], pos, Quaternion.identity);
            
        //    yield return null;
        //}

        for (int i = 0; i < SpawnPoints.transform.childCount; i++) {
            Vector3 pos = SpawnPoints.transform.GetChild(i).position;

            int index = Random.Range(0, prefabs.Count - 1);
            Instantiate(prefabs[index], pos, Quaternion.identity);

            yield return null;
        }
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
