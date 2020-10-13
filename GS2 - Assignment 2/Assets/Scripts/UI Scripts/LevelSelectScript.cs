using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScript : MonoBehaviour
{
    public Text levelText;
    public Text totalEnemiesText;
    public Text bestTimeText;
    
    private GameObject parentCanvas;
    private string levelSelected;
    private GameObject previousCanvas;

    private AudioClip hoverSound;
    private AudioClip clickSound;

    private AudioSource audioSource;

    public GameObject PreviousCanvas {
        get { return previousCanvas; }
        set { previousCanvas = value; }
    }

    private void Start() {
        parentCanvas = transform.root.gameObject;

        hoverSound = (AudioClip)Resources.Load("Sounds/click1");
        clickSound = (AudioClip)Resources.Load("Sounds/rollover1");

        audioSource = GetComponent<AudioSource>();
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
        displayStats();
    }
     
    private void displayStats() {
        float bestTime = PlayerPrefs.GetFloat(levelSelected, -1f);

        levelText.text = levelSelected;
        if (bestTime != -1f)
            bestTimeText.text = "Best Time: " + bestTime.ToString("f1");
        else
            bestTimeText.text = "Not Yet Completed";


    }
    public void playHoverSound() {
        audioSource.PlayOneShot(hoverSound);
    }

    public void playClickSound() {
        audioSource.PlayOneShot(clickSound);
    }
}
