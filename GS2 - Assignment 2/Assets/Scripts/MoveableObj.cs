using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObj : MonoBehaviour
{
    public Transform point0, point1;
    public float moveTime;
    private Vector3 lastMove;
    private bool activated;
    private bool finishedMove;
    public PlayerMovement player;
    
    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        finishedMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startMovement()
    {
        if (!activated)
        {
            UnityEngine.Debug.Log("activating moveable obj");
            activated = true;
            StartCoroutine(moveObj());
        }
    }

    public Vector3 getLastMove()
    {
        return lastMove;
    }

    public bool getFinishedMove()
    {
        return finishedMove;
    }

    private IEnumerator moveObj()
    {
        float elapsedTime = 0;
        Vector3 startPos = point0.position;
        Vector3 lastPos = startPos;
        Vector3 nextPos = startPos;
        while (elapsedTime < moveTime)
        {
            lastPos = nextPos;
            nextPos = Vector3.Lerp(startPos, point1.position, elapsedTime / moveTime);
            this.transform.position = nextPos;
            lastMove = nextPos - lastPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        finishedMove = true;
    }
}
