using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    private float timer;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        text = GetComponent<Text>();

        //Reset timer when new level is loaded
        GameManager.Instance.timer = 0f;
        GameManager.Instance.isTiming = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer = GameManager.Instance.timer;
        string minutes = ((int)timer / 60).ToString();
        string seconds = (timer % 60).ToString("f1");

        text.text = minutes + ":" + seconds;        

    }
}
