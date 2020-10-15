using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerReductionScript : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
    //    TextMeshProUGUI tmPro = GetComponent<TextMeshProUGUI>();
    //    tmPro.text = GameManager.Instance.timerReduction.ToString();
    //}

    public void destoryThis() {
        Destroy(gameObject);
    }
}
