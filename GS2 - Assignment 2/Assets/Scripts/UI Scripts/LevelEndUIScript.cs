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
    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("isMoving", true);
        setText();
    }

    public void buttonAction(GameObject buttonPressed) {
        
        switch (buttonPressed.name) {
            case "Retry Button":
                Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 1f);
                break;
            case "Next Level Button":
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                //print("NEXT SCENE INDEXX: " + nextSceneIndex);

                //string nextSceneName = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
                string nextLevelName = "Level " + (SceneManager.GetActiveScene().buildIndex + 1);
                Initiate.Fade(nextLevelName, Color.black, 1f);
                break;
            case "Main Menu Button":
                Initiate.Fade("MainMenu", Color.black, 1f);
                break;
        }
    }
    
    private void setText() {
        //Set currentScore Text
        string currentScoreMinutes = ((int)GameManager.Instance.timer / 60).ToString();
        string currentScoreSeconds = (GameManager.Instance.timer % 60).ToString("f1");
        print("HIGH SCOE" + currentScoreSeconds);
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
    }

}
