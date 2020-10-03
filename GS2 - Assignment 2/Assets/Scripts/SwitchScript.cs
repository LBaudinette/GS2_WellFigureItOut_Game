﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    private bool activated;
    public GameObject thisSwitch;
    public GameObject parentObj;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enable()
    {
        if (!activated)
        {
            activated = true;
            thisSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            parentObj.GetComponent<LevelEndScript>().switchActivated();
        }
    }
}
