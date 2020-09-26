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

        //unlock cursor in case we loaded from game scene
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void buttonAction(GameObject buttonPressed) {
        switch (buttonPressed.name) {
            case "Play Button":
                Initiate.Fade("Level 1", Color.black, 1.0f);
                break;
            case "Level Select Button":
                Instantiate(selectLevelPrefab);
                //set the canvas that the 'back' button will load back to
                GameObject.Find("Level Select Canvas(Clone)/Button Selector").GetComponent<LevelSelectScript>().PreviousCanvas
                    = (GameObject)Resources.Load("Menu Canvas Prefabs/Main Menu Canvas"); 
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
