using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused, isTiming, levelFinished;
    public float timer = 0f;
    public int enemiesDefeated = 0;
    private static GameManager instance = null;
    private GameObject pauseCanvas;
    private GameObject pauseCanvasClone;
    

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
            if (timer < savedTime)
                PlayerPrefs.SetFloat(currentScene, timer);
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
    }
    public void reduceTimer(float amount) {
        timer -= amount;
    }
    private void updateTimer() {
        if (!isPaused && isTiming) 
            timer += Time.deltaTime;
    }

    
}
