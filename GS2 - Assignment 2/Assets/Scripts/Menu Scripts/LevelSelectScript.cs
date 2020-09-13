using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScript : MonoBehaviour
{
    //Canvas for the main menu screen
    public GameObject mainMenuPrefab;
    //Canvas that the Button Select game object is a child of
    public GameObject parentCanvas;
    private string levelSelected;
    public void buttonAction(GameObject buttonPressed) {
        
        switch (buttonPressed.name) {
            case "Main Menu Button":
                Instantiate(mainMenuPrefab);
                Destroy(parentCanvas);
                break;
            case "Continue Button":
                SimpleSceneFader.ChangeSceneWithFade(levelSelected);
                break;
            case "Level 1 Button":
                levelSelected = "Level 1";
                break;
            case "Level 2 Button":
                levelSelected = "Level 2";
                break;
            case "Level 3 Button":
                levelSelected = "Level 3";
                break;
            case "Level 4 Button":
                levelSelected = "Level 4";
                break;
            case "Level 5 Button":
                levelSelected = "Level 5";
                break;

        }
    }
}
