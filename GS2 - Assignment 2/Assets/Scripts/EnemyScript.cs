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
        player = GameObject.FindGameObjectWithTag("Player");

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
        Vector3 playerDir;
        if(gameObject.name == "Air Enemy"){
            playerDir = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        }
        else {
            playerDir = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
        transform.LookAt(player.transform, Vector3.up);
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerDir, step);

    }

    void checkForPlayer() {
        Vector3 distance = player.transform.position - transform.position;
        if(distance.magnitude < aggroDistance) {
            isChasing = true;
        }

    }
}
