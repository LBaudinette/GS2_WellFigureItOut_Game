using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    //Canvas for select level screen
    public GameObject selectLevelPrefab;
    //Canvas that the Button Select game object is a child of
    private GameObject parentCanvas;

    private void Start() {
        parentCanvas = transform.root.gameObject;
    }
    public void buttonAction(GameObject buttonPressed) {
        switch (buttonPressed.name) {
            case "Play Button":
                SimpleSceneFader.ChangeSceneWithFade("Level 1");
                break;
            case "Level Select Button":
                Instantiate(selectLevelPrefab);
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
