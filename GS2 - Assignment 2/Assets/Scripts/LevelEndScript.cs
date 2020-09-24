using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndScript : MonoBehaviour
{
    private bool enabled;
    public int numSwitches;
    public GameObject levelEnd;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchActivated()
    {
        numSwitches--;
        UnityEngine.Debug.Log("num switches = " + numSwitches);
        if (numSwitches == 0)
        {
            enabled = true;
            levelEnd.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    public void nextLevel()
    {
        if (enabled)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.Equals("Level 1")) {
                SceneManager.UnloadSceneAsync("Level 1");
                SceneManager.LoadScene("Level 2");
            }
            else if (sceneName.Equals("Level 2")) {
                SceneManager.UnloadSceneAsync("Level 2");
                SceneManager.LoadScene("Level 3");
            }
            else if (sceneName.Equals("Level 3")) {
                SceneManager.UnloadSceneAsync("Level 3");
                SceneManager.LoadScene("Level 4");
            }
            else if (sceneName.Equals("Level 4")) {
                SceneManager.UnloadSceneAsync("Level 4");
                SceneManager.LoadScene("Level 5");
            }
            else if (sceneName.Equals("Level 5"))
            {
                SceneManager.UnloadSceneAsync("Level 5");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
