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
            if (SceneManager.GetActiveScene().name.Equals("Level 1")){
                SceneManager.UnloadSceneAsync("Level 1");
                SceneManager.LoadScene("Level 2");
            }
        }
    }
}
