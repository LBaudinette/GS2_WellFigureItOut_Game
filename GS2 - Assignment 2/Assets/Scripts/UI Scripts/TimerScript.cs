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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isPaused) {

            timer += Time.deltaTime;

            string minutes = ((int)timer / 60).ToString();
            string seconds = (timer % 60).ToString("f1");

            text.text = minutes + ":" + seconds;
        }

    }
}
