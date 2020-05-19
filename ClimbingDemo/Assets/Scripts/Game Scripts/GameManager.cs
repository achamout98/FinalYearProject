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
    public GameObject VictoryScreen;
    public static bool is_paused = false;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject RockSpawnPoints;

    [SerializeField]
    private List<GameObject> RockPrefabs;

    [Header("References")]
    [SerializeField]
    private GameObject Cam;
    [SerializeField]
    private GameObject Player;

    public static GameManager instance;
    public bool mountainConquered = false;

    private bool VictoryTextGone = false;

    private void Awake () {
        instance = this;
        Cam = Camera.main.gameObject;
        Resume();
        StartCoroutine("SpawnRocks");
    }

    // Start is called before the first frame update
    void Start()
    {
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
        if (mountainConquered) {
            VictoryScreen.SetActive(true);
            VictoryTextTing();
            StartCoroutine(EndGame());//AltPause();
            mountainConquered = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (is_paused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void VictoryTextTing () {
        StartCoroutine(RemoveVictoryScreen());
    }

    private IEnumerator RemoveVictoryScreen() {
        Debug.Log("start");
        var newPos = new Vector3(-1000, VictoryScreen.transform.position.y, VictoryScreen.transform.position.z);

        float time = 7f;
        float eta = 0f;

        yield return new WaitForSecondsRealtime(3f);
        while(eta < time) {
            VictoryScreen.transform.position = Vector3.Lerp(VictoryScreen.transform.position, newPos, (eta/time));
            eta += Time.deltaTime;
            yield return null;
        }
        VictoryTextGone = true;
        Debug.Log("done");
        yield break;
    }

    public void Quit () {
        Time.timeScale = 1;
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

    IEnumerator EndGame () {
        yield return new WaitUntil(() => VictoryTextGone);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(10f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield break;
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
