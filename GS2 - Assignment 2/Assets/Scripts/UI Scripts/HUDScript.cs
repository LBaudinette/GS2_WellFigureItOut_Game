using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public Text timerText;
    public GameObject jumpCounter;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        //Reset timer when new level is loaded
        GameManager.Instance.timer = 0f;
        GameManager.Instance.isTiming = true;

    }

    // Update is called once per frame
    void Update()
    {
        //When level is finished, disable the hud
        if (GameManager.Instance.levelFinished)
            Destroy(gameObject);

        setJumpCounters();
        timer = GameManager.Instance.timer;
        string minutes = ((int)timer / 60).ToString();
        string seconds = (timer % 60).ToString("f1");

        timerText.text = minutes + ":" + seconds;        

    }

    private void setJumpCounters() {
        int numJumps =
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().jumpCounter;
        //switch (numJumps) {
        //    case 0:

        //        break;
        //    case 1:
        //        break;
        //    case 2:
        //        break;
        //}
        if (numJumps == 0)
            jumpCounter.SetActive(false);
        else
            jumpCounter.SetActive(true);
    }

}
