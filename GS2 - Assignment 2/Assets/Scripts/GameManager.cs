using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused, isTiming, levelFinished, isHighScore, isFrozen;
    public float timer = 0f;
    public int enemiesDefeated = 0;
    private static GameManager instance = null;
    private GameObject pauseCanvas;
    private GameObject pauseCanvasClone;
    private AudioSource musicSource;
    private AudioClip music;
    private float freezeTimer = 0f;
    public int timerReduction = 2;



    public static GameManager Instance {
        get {
            if (instance == null) {
                GameObject GameManagerObject = new GameObject();
                GameManagerObject.name = "GameManager";

                instance = GameManagerObject.AddComponent<GameManager>();
                
            }
            return instance;
        }

    }


    void Awake() {
        DontDestroyOnLoad(gameObject);
        music = (AudioClip)Resources.Load("wave_test_2");
        gameObject.AddComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();
        musicSource.volume = 0.6f;
        musicSource.loop = true;
        musicSource.clip = music;
        musicSource.Play();
        pauseCanvas = (GameObject)Resources.Load("UI/Pause Canvas");
        isPaused = false;
        levelFinished = false;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
        if (freezeTimer >= 0f) {
            isFrozen = true;
            freezeTimer -= Time.deltaTime;
            print(freezeTimer);
        }
        else
            isFrozen = false;
    }
    //isPausedMenu is true if we are pausing the game through the pause menu
    public void pauseGame(bool isPausedMenu) {
        
        isPaused = true;
        isTiming = false;
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseCanvasClone = Instantiate(pauseCanvas);
    }
    public void unPauseGame() {
        //Destroy any menu canvases that are open
        Destroy(GameObject.Find("Level Select Canvas(Clone)"));
        Destroy(GameObject.Find("Pause Canvas(Clone)"));

        isPaused = false;
        if(!levelFinished)
            isTiming = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void finishTime() {
        isTiming = false;
        levelFinished = true;
        //pauseGame(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        string currentScene = SceneManager.GetActiveScene().name;
        float savedTime =
             PlayerPrefs.GetFloat(currentScene, -1f);
        if(savedTime != -1f) {
            if (timer < savedTime) {
                PlayerPrefs.SetFloat(currentScene, timer);
                isHighScore = true;
            }
                
        }
        else {
            PlayerPrefs.SetFloat(currentScene, timer);
        }

        //Instantiate the Level End Screen
        Instantiate((GameObject)Resources.Load("UI/Level End Canvas"));
    }

    public void resetStats() {
        timer = 0f;
        enemiesDefeated = 0;
        isHighScore = false;
    }
    //Method used to reduce the timer when defeating enemies
    public void reduceTimer() {
        timer -= timerReduction;

        //show the reduction in time
        //Instantiate(Resources.Load("UI/Timer Reduction Canvas"));

    }
    private void updateTimer() {
        if (!isPaused && isTiming && !isFrozen) 
            timer += Time.deltaTime;
    }

    public void freezeTime() {
        startFreeze();
    }

    private void startFreeze() {
        //isFrozen = true;
        freezeTimer += timerReduction;
        if (freezeTimer >= 0f) {
            //StartCoroutine(stopTimer());

        }


    }

    private IEnumerator stopTimer() {

        if(freezeTimer >= 0f) {
            freezeTimer -= Time.deltaTime;
            print(freezeTimer);
            yield return null;
        }
        print("Done");
        isFrozen = false;

    }
}
