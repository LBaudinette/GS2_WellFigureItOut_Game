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
    private bool isChasing, inEnemyZone;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemyZone")
        {
            UnityEngine.Debug.Log("entered enemy zone");
            inEnemyZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemyZone")
        {
            UnityEngine.Debug.Log("left enemy zone");
            inEnemyZone = false;
        }
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
        if (inEnemyZone)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerDir, step);
        }
        

    }

    void checkForPlayer() {
        Vector3 distance = player.transform.position - transform.position;
        if(distance.magnitude < aggroDistance) {
            isChasing = true;
        }

    }
}
