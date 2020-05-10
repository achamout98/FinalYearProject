using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("UI")]
    public GameObject GameUI;
    public GameObject PauseMenu;
    public GameObject OptionsMenu;
    public static bool is_paused = false;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject RockSpawnPoints;

    [SerializeField]
    private List<GameObject> RockPrefabs;

    [SerializeField]
    private List<GameObject> Walls;

    [SerializeField]
    private GameObject WallSpawnPoints;

    [Header("References")]
    [SerializeField]
    private GameObject Cam;
    [SerializeField]
    private GameObject Player;

        // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnRocks");
    }

    private IEnumerator SpawnRocks () {

        for (int i = 0; i < RockSpawnPoints.transform.childCount; i++) {
            Vector3 pos = RockSpawnPoints.transform.GetChild(i).position;

            int index = Random.Range(0, RockPrefabs.Count - 1);
            Instantiate(RockPrefabs[index], pos, Quaternion.identity);

            yield return null;
        }
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (is_paused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Quit () {
        SceneManager.LoadScene("Menu");
    }

    void Pause () {
        is_paused = true;
        Cam.GetComponent<CameraController>().enabled = false;
        GameUI.SetActive(false);
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Resume () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        is_paused = false;
        Cam.GetComponent<CameraController>().enabled = true;
        PauseMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1;
    }
}
