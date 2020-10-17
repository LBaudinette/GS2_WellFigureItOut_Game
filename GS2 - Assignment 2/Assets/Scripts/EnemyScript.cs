using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed, aggroDistance;
    public LayerMask enemyZoneMask;
    private CharacterController charController;
    private float angle;
    private Quaternion currentRotation;
    private bool isChasing, inEnemyZone;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        inEnemyZone = true;
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
        Vector3 nextPos = Vector3.MoveTowards(transform.position, playerDir, step);
        if (gameObject.name == "Air Enemy"){
            if (Physics.CheckSphere(nextPos, 0.1f, enemyZoneMask))
            {

                transform.position = nextPos;
            }
        } else
        {
            transform.position = nextPos;
        }
        

    }

    void checkForPlayer() {
        Vector3 distance = player.transform.position - transform.position;
        if(distance.magnitude < aggroDistance) {
            isChasing = true;
        }

    }
}
