﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScript : MonoBehaviour
{
    //Canvas for the main menu screen
    //public GameObject mainMenuPrefab;
    private GameObject pauseMenuPrefab;
    
    private GameObject parentCanvas;
    private string levelSelected;

    private GameObject previousCanvas;
    public GameObject PreviousCanvas {
        get { return previousCanvas; }
        set { previousCanvas = value; }
    }

    private void Start() {
        parentCanvas = transform.root.gameObject;
        print("GAME OBJECT: " + gameObject.name);
        print(transform.root.name);
    }
    private void Update() {
       
    }
    public void buttonAction(GameObject buttonPressed) {
        switch (buttonPressed.name) {
            
            case "Back Button":
                Instantiate(previousCanvas);
                Destroy(parentCanvas);
                break;
            case "Continue Button":
                if(GameManager.Instance.isPaused)
                    GameManager.Instance.unPauseGame();
                Initiate.Fade(levelSelected, Color.black, 1.0f);
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
