using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed, aggroDistance;
    private CharacterController charController;
    private float angle;
    private Quaternion currentRotation;
    private bool isChasing;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChasing)
            checkForPlayer();
        else
            moveToPlayer();
    }

    void moveToPlayer() {
        transform.LookAt(player.transform, Vector3.up);
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);

    }

    void checkForPlayer() {
        Vector3 distance = player.transform.position - transform.position;
        if(distance.magnitude < aggroDistance) {
            isChasing = true;
        }

    }
}
