using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public Text timerText;
    public GameObject jumpCounter;
    public Text enemyCounterText;
    private float timer;
    private int maxEnemies;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        //Reset timer when new level is loaded
        GameManager.Instance.isTiming = true;
        setEnemyCount();

    }

    // Update is called once per frame
    void Update()
    {
        //When level is finished, disable the hud
        if (GameManager.Instance.levelFinished)
            Destroy(gameObject);

        setJumpCounters();
        updateEnemyCount();

        timer = GameManager.Instance.timer;
        string minutes = ((int)timer / 60).ToString();
        string seconds = (timer % 60).ToString("f1");

        timerText.text = minutes + ":" + seconds;        

    }


    private void setJumpCounters() {
        int numJumps =
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().jumpCounter;
  
        if (numJumps == 0)
            jumpCounter.SetActive(false);
        else
            jumpCounter.SetActive(true);
    }
    private void setEnemyCount() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        maxEnemies = enemies.Length;
        enemyCounterText.text = GameManager.Instance.enemiesDefeated.ToString() +
            "/" + maxEnemies.ToString();
    }

    private void updateEnemyCount() {

        enemyCounterText.text = GameManager.Instance.enemiesDefeated.ToString() +
            "/" + maxEnemies.ToString();
    }

}
