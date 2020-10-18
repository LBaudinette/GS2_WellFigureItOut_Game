using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndUIScript : MonoBehaviour
{

    public Text highScoreText;
    public Text currentScoreText;
    public Animator animator;
    public GameObject newHighScore;

    private AudioClip hoverSound;
    private AudioClip clickSound;

    private AudioSource audioSource;

    void Start()
    {
        animator.SetBool("isMoving", true);
        setText();

        hoverSound = (AudioClip)Resources.Load("Sounds/click1");
        clickSound = (AudioClip)Resources.Load("Sounds/rollover1");

        audioSource = GetComponent<AudioSource>();

    }

    public void buttonAction(GameObject buttonPressed) {
        
        switch (buttonPressed.name) {
            case "Retry Button":
                Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 1f);
                break;
            case "Next Level Button":
                if (SceneManager.GetActiveScene().buildIndex == 6)
                {
                    Initiate.Fade("MainMenu", Color.black, 1f);
                } else
                {
                    int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                    //print("NEXT SCENE INDEXX: " + nextSceneIndex);

                    //string nextSceneName = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
                    string nextLevelName = "Level " + (SceneManager.GetActiveScene().buildIndex);
                    Initiate.Fade(nextLevelName, Color.black, 1f);
                }
                break;
            case "Main Menu Button":
                Initiate.Fade("MainMenu", Color.black, 1f);
                break;
        }

        GameManager.Instance.resetStats();

    }
    
    private void setText() {
        //Set currentScore Text
        string currentScoreMinutes = ((int)GameManager.Instance.timer / 60).ToString();
        string currentScoreSeconds = (GameManager.Instance.timer % 60).ToString("f1");
        currentScoreText.text = "Your Time: \n" + currentScoreMinutes + ":" + currentScoreSeconds;

        //Set high score text
        float highScore = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name, -1);
        if (highScore != -1) {

            string highScoreMinutes = ((int)highScore / 60).ToString();
            string highScoreSeconds = (highScore % 60).ToString("f1");

            highScoreText.text = "Best Time: \n" + highScoreMinutes + ":" + highScoreSeconds;
        }
        else {
            highScoreText.text = "Best Time: \n" + currentScoreText.text;
        }
        //if the player has a new high score, enable the "New High Score" text
        if (GameManager.Instance.isHighScore)
            newHighScore.SetActive(true);
    }

    public void playHoverSound() {
        audioSource.PlayOneShot(hoverSound);
    }

    public void playClickSound() {
        audioSource.PlayOneShot(clickSound);
    }
}
