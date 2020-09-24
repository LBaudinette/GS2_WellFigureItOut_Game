using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public bool isPaused;

    public static GameManager Instance {
        get {
            if (instance == null) {
                GameObject GameManagerObject = new GameObject();
                GameManagerObject.name = "GameManager";

                instance = GameManagerObject.AddComponent<GameManager>();
            }
            return instance;
        }

    }


    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
