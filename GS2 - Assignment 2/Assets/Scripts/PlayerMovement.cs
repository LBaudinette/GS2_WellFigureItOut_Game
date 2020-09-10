using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed, slopeForce, jumpForce, groundDistance;
    public LayerMask groundMask;
    public Transform groundCheck, camera;
    private bool isGrounded;
    private CharacterController charController;
    private PlayerLook playerLook;
    private bool isWallRunning, isJumping, isCrouching, isSprinting;
    private Animator animator;
    private int? currentWall = 0, lastWall = 0;
    private float gravity = -9.81f; //default value of gravity in Unity
    private float walkSpeed = 3f;
    private float sprintSpeed = 7f;
    private float crouchTime = 2f;
    private float crouchHeight = 1.5f;
    private float standingHeight = 2.0f;
    private float timer = 0.0f;
    private Coroutine crouchHeightChange = null;
    private Coroutine currentRoutine;
    Vector3 velocity; // Used for gravity



    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerLook = GetComponent<PlayerLook>();
    }

    // Update is called once per frame
    void Update()
    {
        checkWall();

        if (lastWall != null) {
            if (timer >= 5.0) {
                timer = 0.0f;
                lastWall = null;
            }
            timer += Time.deltaTime;
        }
        movement();

    }

    //Jumping and gravity obtained from: https://youtu.be/_QajrabyTJc
    private void movement() {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0) {
            isJumping = false;
            velocity.y = -2.0f;
        }
        
        if (Input.GetButtonDown("Sprint"))
        {
            enterSprint();
        }
        if (Input.GetButtonUp("Sprint")){
            exitSprint();
        }

        move();

        //General gravity always applied to player when not wall running
        if (!isWallRunning) {
            velocity.y += gravity * Time.deltaTime;
        }
        else {
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
        }

        if (Input.GetButtonDown("Crouch") && isGrounded)
        {
            this.isCrouching = true;
            changeCrouchHeight();
        } 
        if (Input.GetButtonUp("Crouch") && isGrounded)
        {
            this.isCrouching = false;
            changeCrouchHeight();
        }


        if (onSlope()) {
            print("slope");
            charController.Move(Vector3.down * slopeForce * Time.deltaTime);
        }

        charController.Move(velocity * Time.deltaTime);

        
    }

    //checks if the player is on a slope
    private bool onSlope() {
        RaycastHit hit;

        //if the normal of the surface the player is standing on is not pointing up, then it is a slope
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10)) {
            if(hit.normal != Vector3.up) {
                
                return true;
            }
        }
        return false;
    }
    private void move()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0.0f, Input.GetAxisRaw("Vertical") * speed);
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        charController.Move(movement);
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
        //if(hitLeft.collider != null && hitRight.collider != null) {
        //    if (checkWall(hitLeft) && checkWall(hitRight)) {
        //        enterWallRun(getLongestHit(hitLeft, hitRight));
        //        return;
        //    }
        //}

        if (hitLeft.collider != null &&
            checkHit(hitLeft) &&
            hitLeft.collider.gameObject.GetInstanceID() != lastWall) {
            if (!isWallRunning) {

                if (currentRoutine != null)
                    StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(rotateCamera(-20.0f));
            }
            enterWallRun(hitLeft);
        }
        else if (hitRight.collider != null &&
            checkHit(hitRight) &&
            hitRight.collider.gameObject.GetInstanceID() != lastWall) {

            if (!isWallRunning) {
                if (currentRoutine != null)
                    StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(rotateCamera(20.0f));
            }
            enterWallRun(hitRight);

        }
        else { //if there are no valid walls to either side of the player
            if (isWallRunning) {
                exitWallRun();
            }


            return;
        }
    }
    bool checkHit(RaycastHit hit) {
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

    void enterWallRun(RaycastHit wall) {
        currentWall = wall.collider.gameObject.GetInstanceID();
        isWallRunning = true;

    }
    void exitWallRun() {
        isWallRunning = false;
        lastWall = currentWall;
        currentWall = null;
        if (currentRoutine != null) {
            StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(resetCamera());
        }
    }
    IEnumerator rotateCamera(float angle) {
        print("ROTATION: " + camera.transform.eulerAngles.z);

        Vector3 currentRotation = camera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
        Vector3 currentRotate;
        print("CURRENT: " + currentRotation + " TARGET: " + targetRotation);
        float duration = 0.3f;
        float time = 0.0f;

        while (time < duration) {

            currentRotate = Vector3.Lerp(currentRotation, targetRotation, time / duration);
            camera.transform.eulerAngles = currentRotate;
            //camera.transform.rotation = Quaternion.Euler(currentRotate);

            time += Time.deltaTime;

            yield return null;
        }

        camera.transform.eulerAngles = targetRotation;
    }
    IEnumerator resetCamera() {
        float targetZ = (camera.transform.eulerAngles.z > 180.0f) ? 360.0f : 0.0f;

        Vector3 currentRotation = camera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, targetZ);
        Vector3 currentRotate;
        float duration = 0.3f;
        float time = 0.0f;

        while (time < duration) {

            currentRotate = Vector3.Lerp(currentRotation, targetRotation, time / duration);
            camera.transform.rotation = Quaternion.Euler(currentRotate);

            time += Time.deltaTime;

            yield return null;
        }


        camera.transform.eulerAngles = targetRotation;


    }

    private void enterSprint()
    {
        isSprinting = true;
        speed = sprintSpeed;
        playerLook.sprint(isSprinting);
    }

    private void exitSprint()
    {
        isSprinting = false;
        speed = walkSpeed;
        playerLook.sprint(isSprinting);
    }

    private void changeCrouchHeight()
    {
        if (crouchHeightChange != null)
        {
            StopCoroutine(crouchHeightChange);
        }
        crouchHeightChange = StartCoroutine(crouchLerp());
    }

    private IEnumerator crouchLerp()
    {
        float elapsedTime = 0;
        // set change based on whether player is crouching
        float endY = this.isCrouching ? crouchHeight : standingHeight;

        while (elapsedTime < crouchTime)
        {
            charController.height = Mathf.Lerp(charController.height, endY, elapsedTime / crouchTime);
            UnityEngine.Debug.Log(charController.height);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}

