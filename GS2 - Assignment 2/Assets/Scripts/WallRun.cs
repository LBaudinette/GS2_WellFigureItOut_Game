using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{

    private CharacterController charController;
    private bool isWallRunning = false;

    public float speed, slopeForce, jumpForce, groundDistance;
    public LayerMask groundMask;
    public Transform groundCheck, camera;
    private bool isGrounded;
    private float timer = 0.0f;
    private bool canWallrun = true;
    private bool isJumping = false;
    private int? currentWall = 0, lastWall = 0;
    private Vector3? currentWallNormal = null, lastWallNormal = null;
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

        checkWall();

        if(lastWall != null) {
            if (timer >= 1.0) {
                timer = 0.0f;
                lastWall = null;
            }
            timer += Time.deltaTime;
        }
            
        
    
    //if (isWallRunning)
    //    wallRun(currentWall);
    //else
    movement();
        //print(timer);
    }


    void checkWall() {
        RaycastHit hitLeft, hitRight;
        Vector3 left = transform.TransformDirection(Vector3.left);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float raycastLength = 1.0f;

        Physics.Raycast(transform.position, left, out hitLeft, raycastLength);
        UnityEngine.Debug.DrawRay(transform.position, left, Color.blue);

        Physics.Raycast(transform.position, right, out hitRight, raycastLength);
        UnityEngine.Debug.DrawRay(transform.position, right, Color.red);


        //Check is there are walls on both sides of the player 
        if(hitLeft.collider != null && hitRight.collider != null) {
            if (checkWall(hitLeft) && checkWall(hitRight)) {
                enterWallRun(getLongestHit(hitLeft, hitRight));
                return;
            }
        }
        
        if(hitLeft.collider != null && 
            checkWall(hitLeft) && 
            hitLeft.collider.gameObject.GetInstanceID() != lastWall) {

            if (!isWallRunning) {
                print("YES");
                StartCoroutine(rotateCameraLeft());

            }

            enterWallRun(hitLeft);
        }
        else if (hitRight.collider != null && 
            checkWall(hitRight) && 
            hitRight.collider.gameObject.GetInstanceID() != lastWall) {
            camera.transform.Rotate(camera.transform.rotation.x, camera.transform.rotation.y, 15.0f);

            enterWallRun(hitRight);

        }
        else { //if there are no valid walls to either side of the player
            if(isWallRunning)
                exitWallRun();
            return;
        }
    }

    IEnumerator rotateCameraLeft() {
        print("ROTATION: " + camera.transform.eulerAngles.z);
        Vector3 currentRotation = camera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, -20.0f);
        Vector3 currentRotate;
        print("CURRENT: " + currentRotation + " TARGET: " + targetRotation);
        float duration = 0.5f;
        float time = 0.0f;
        float value = 0.0f;

        while (time < duration) {

            currentRotate = Vector3.Lerp(currentRotation, targetRotation,time / duration);
            camera.transform.eulerAngles = currentRotate;

            time += Time.deltaTime;

            yield return null;
        }


        camera.transform.eulerAngles = targetRotation;
    }
    private void movement() {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            isJumping = false;
            velocity.y = 0.0f;
        }
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0.0f, Input.GetAxisRaw("Vertical") * speed);

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        charController.Move(movement);

        

        //General gravity always applied to player when not wall running
        if (!isWallRunning) {
            velocity.y += gravity * Time.deltaTime;
        }
        else{
            velocity.y = 0.0f;
        }
        //Jump Function using equation for gravity potential energy
        if (Input.GetButtonDown("Jump")) {
            isJumping = true;
            //StartCoroutine(rotateCameraLeft());

            //If player jumps while wallrunning, stop any wallrunning
            if (isWallRunning) {
                exitWallRun();
            }
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            //sqrt -2*f*g
        }

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

    void enterWallRun(RaycastHit wall) {
        //print("CURRENT: " + currentWall + " LAST NORMAL: " + lastWall);
        currentWall = wall.collider.gameObject.GetInstanceID();
        isWallRunning = true;

    }
    void exitWallRun() {
        isWallRunning = false;
        lastWall = currentWall;
        currentWall = null;
        
        //print(" NEW CURRENT: " + currentWall + " NEW LAST NORMAL: " + lastWall);

    }

    //Move along the surface that the player is current touching
    void wallRun(RaycastHit wall) {
        Vector3 wallVector = Vector3.Cross(wall.normal, Vector3.up);
        wallVector *= speed;
        wallVector *= Time.deltaTime;

        //wallVector = transform.TransformDirection(wallVector);
        //Move forward on the Vector along the wall
        if (Input.GetKey(KeyCode.W)) {
            charController.Move(wallVector);
        }

        print(wallVector);
    }


    //Checks if the object is a wall
    bool checkWall(RaycastHit hit) {
        if (hit.collider.gameObject.tag == "Wall") {
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

