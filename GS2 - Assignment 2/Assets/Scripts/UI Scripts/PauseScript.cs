using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    //Canvas for select level screen
    public GameObject selectLevelPrefab;

    //Canvas for Main Menu screen
    public GameObject mainMenuPrefab;

    //Canvas that the Button Select game object is a child of
    private GameObject parentCanvas;

    private void Start() {
        parentCanvas = transform.root.gameObject;
    }
    public void buttonAction(GameObject buttonPressed) {
        switch (buttonPressed.name) {
            case "Resume Button":
                GameManager.Instance.unPauseGame();

                Destroy(parentCanvas);
                break;
            case "Retry Button":
                GameManager.Instance.unPauseGame();
                Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 1f);
                break;
            case "Level Select Button":
                Instantiate(selectLevelPrefab);
                GameObject.Find("Level Select Canvas(Clone)/Button Selector").GetComponent<LevelSelectScript>().PreviousCanvas
                    = (GameObject)Resources.Load("UI/Pause Canvas");
                Destroy(parentCanvas);
                break;
            case "Main Menu Button":
                GameManager.Instance.unPauseGame();
                Initiate.Fade("MainMenu", Color.black, 1f);

                break;
            case "Quit Game Button":
                //If the game is running in Unity, different quit function is needed
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;



        }

    }
}
