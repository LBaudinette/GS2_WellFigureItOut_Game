using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    private bool enabled;
    public GameObject thisSwitch;
    public GameObject parentObj;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        thisSwitch.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enable()
    {
        if (!enabled)
        {
            enabled = true;
            Debug.Log("EMISSION");
            thisSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            //thisSwitch.GetComponent<Renderer>().material.;

            parentObj.GetComponent<LevelEndScript>().switchActivated();
        }
    }
}
