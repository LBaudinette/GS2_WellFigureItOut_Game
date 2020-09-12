using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public Vector3 forceDir;
    public bool padEnabled;
    public float forceSpeed, forceTime;

    // Start is called before the first frame update
    void Start()
    {
        if (padEnabled)
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
