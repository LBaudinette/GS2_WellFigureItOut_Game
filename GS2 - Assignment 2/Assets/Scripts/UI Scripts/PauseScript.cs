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
                Destroy(parentCanvas);
                break;
            case "Retry Button":
                //Reload scene 
                SimpleSceneFader.ChangeSceneWithFade(SceneManager.GetActiveScene().name);
                break;
            case "Level Select Button":
                Instantiate(selectLevelPrefab);
                Destroy(parentCanvas);
                break;
            case "Main Menu Button":
                Instantiate(mainMenuPrefab);
                Destroy(parentCanvas);
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
