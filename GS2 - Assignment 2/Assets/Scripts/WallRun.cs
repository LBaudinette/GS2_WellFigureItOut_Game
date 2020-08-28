using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{

    private CharacterController charController;
    private bool isWallRunning;

    public float speed, slopeForce, jumpForce, groundDistance;
    public LayerMask groundMask;
    public Transform groundCheck;
    private bool isGrounded;
    private bool isJumping;
    private Animator animator;
    private float gravity = -9.81f; //default value of gravity in Unity
    public Vector3 velocity; // Used for gravity

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        //checkWall();
        //if (isWallRunning)
        //    wallRun();
        //else
            movement();
    }


    void checkWall() {
        RaycastHit hitLeft, hitRight;
        Vector3 left = transform.TransformDirection(Vector3.left);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //Debug.Log("Left " + left + "  Right: " + right);
        //Debug.Log(transform.rotation.y);
        float raycastLength = 1.0f;

        Physics.Raycast(transform.position, left, out hitLeft, raycastLength);
        UnityEngine.Debug.DrawRay(transform.position, left, Color.blue);

        Physics.Raycast(transform.position, right, out hitRight, raycastLength);
        UnityEngine.Debug.DrawRay(transform.position, right, Color.red);


        //Check is there are walls on both sides of the player 
        if(hitLeft.collider != null && hitRight.collider != null) {
            if (checkWall(hitLeft) && checkWall(hitRight))
                wallRun(getLongestHit(hitLeft, hitRight));
        }
        else if(hitLeft.collider != null && checkWall(hitLeft)) {
            wallRun(hitLeft);
        }
        else if (hitRight.collider != null && checkWall(hitRight)) {
            wallRun(hitRight);
        }
        else { //if there are no walls to either side of the player
            isWallRunning = false;
            return;
        }

        


    }

    private void movement() {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0.0f, Input.GetAxisRaw("Vertical") * speed);

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        charController.Move(movement);

        //Jump Function using equation for gravity potential energy
        if (Input.GetButtonDown("Jump")) {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        //General gravity always applied to player
        velocity.y += gravity * Time.deltaTime;

        if (onSlope()) {
            charController.Move(Vector3.down * slopeForce * Time.deltaTime);
        }

        charController.Move(velocity * Time.deltaTime);


    }

    private bool onSlope() {
        RaycastHit hit;

        //if the normal of the surface the player is standing on is not pointing up, then it is a slope
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10)) {
            if (hit.normal != Vector3.up) {

                return true;
            }
        }
        return false;
    }

    void wallRun(RaycastHit wall) {
        isWallRunning = true;
        Vector3 wallVector = Vector3.Cross(wall.normal, Vector3.up);

        //Move forward along
        if(Input.GetKey(KeyCode.W))
        
        print(wallVector);
    }
    //Checks if the object is a wall
    bool checkWall(RaycastHit hit) {
        if (hit.collider.gameObject.tag == "Wall") {
            print("WALL");
            return true;
        }

        //Method suggested by footnotes
        //if (Vector3.Dot(hit.normal, Vector3.up) == 0) {
        //    print("WALL");
        //    return true;
        //}

        return false;
    }

    //Return the raycast with the shortest distance. Used to find which
    //wall is closest to player if near two walls
    RaycastHit getLongestHit(RaycastHit hit1, RaycastHit hit2) {
        if (hit1.distance > hit2.distance)

            return hit2;
        else
            return hit1;
    }


}

