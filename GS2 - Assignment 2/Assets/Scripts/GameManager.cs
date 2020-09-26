using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    private GameObject pauseCanvas;
    private GameObject pauseCanvasClone;
    public bool isPaused;

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
        pauseCanvas = (GameObject)Resources.Load("Menu Canvas Prefabs/Pause Canvas");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pauseGame() {
        pauseCanvasClone = Instantiate(pauseCanvas);
        isPaused = true;
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void unPauseGame() {
        //Destroy any menu canvases that are open
        Destroy(GameObject.Find("Level Select Canvas(Clone)"));
        Destroy(GameObject.Find("Pause Canvas(Clone)"));

        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    

}
