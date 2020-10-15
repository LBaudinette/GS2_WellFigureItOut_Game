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
    public int maxAirJumps;
    public LayerMask groundMask;
    public Transform groundCheck, camera, spawnPoint;
    //Public booleans for gun script
    public bool isMoving, isSprinting, isWallRunning;
    private CharacterController charController;
    private PlayerLook playerLook;
    //public GameObject player;
    private Animator animator;
    private int ? currentWall = 0, lastWall = 0;
    private bool isGrounded, isJumping, isCrouching, isSliding,
        isOnSlope, wasGrounded, wasOnSlope, isPaused;
    private float gravity = -9.81f; //default value of gravity in Unity
    public int jumpCounter = 2;
    private float walkSpeed = 7f;
    private float sprintSpeed = 10f;
    private float airAccel = 5f;
    private float iTimeMax = 1f;
    private float iTime = 0f;

    private float crouchSpeed = 3f;
    private float crouchHeight = 1.25f;
    private float standingHeight = 2.0f;
    private float timer = 0.0f;
    private Coroutine cameraRoutine;

    private float slideSpeed = 15f;
    private float slideTime = 1.5f;
    private float slideHorizontalMoveMult = 0.25f;
    private float slideFriction = 0.3f;
    private float speedDecayRate = 5f;
    private RaycastHit currSlope;
    private Coroutine slideRoutine = null;


    Vector3 velocity; // Used for gravity



    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerLook = GetComponent<PlayerLook>();
        respawn();
        if(GameObject.FindWithTag("HUD") == null){
            print("NEW HUD");
            Instantiate(Resources.Load("UI/HUD"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //do not update if game is paused
        //if (GameManager.Instance.isPaused)
        //    return;

        //Pause Menu
        if (Input.GetButtonDown("Cancel") && !GameManager.Instance.isPaused) {
            GameManager.Instance.pauseGame(true);
        }
        else if (Input.GetButtonDown("Cancel") && GameManager.Instance.isPaused) {
            GameManager.Instance.unPauseGame();

        }

        // decay invincibility time after being hit if iTime is above 0
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
            UnityEngine.Debug.Log("iTime = " + iTime);
        }

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

        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded || isOnSlope)
        {

            resetJumps();

            lastWall = null;
            //UnityEngine.Debug.Log("grounded");
            // check if on slope
            wasOnSlope = isOnSlope;
            isOnSlope = onSlope();

            // reset y velocity if on ground
            if (velocity.y < 0)
            {
                isJumping = false;
                resetGravity();
            }

            // sprinting inputs
            // enter sprint if not sliding/crouching/sprinting and sprint key is pressed and player is moving forward
            //print("isSliding " + isSliding + "isSprinting " + isSprinting + "IsCrouching " + isCrouching);
            if ((!isSliding && !isSprinting && !isCrouching && Input.GetButtonDown("Sprint") && Input.GetAxisRaw("Vertical") > 0) || (!isSprinting  && speed >= sprintSpeed))
            {
                enterSprint();
            } 
            // exit sprint if too slow
            if (isSprinting && (this.speed < sprintSpeed || Input.GetAxisRaw("Horizontal") > 0f || Input.GetAxisRaw("Vertical") == 0f))
            {
                exitSprint();
            }
            
            bool overSprintSpeed = this.speed > sprintSpeed;
            if (isSprinting && overSprintSpeed)
            {
                // if over sprint speed and sprinting and grounded, decay speed over time
                if (speed - speedDecayRate * Time.deltaTime < sprintSpeed)
                {
                    speed = sprintSpeed;
                }
                else if (!isOnSlope)
                {
                    speed -= speedDecayRate * Time.deltaTime;
                }
            }

            // if sprinting, above speed threshold, or on slope, do slide, otherwise crouch
            if (isSprinting || overSprintSpeed)
            {
                if (Input.GetButtonDown("Crouch"))
                {
                    enterSlide();
                }

            } else 
            {
                // crouching inputs
                if (Input.GetButtonDown("Crouch"))
                {
                    enterCrouch();
                }
                if (Input.GetButtonUp("Crouch"))
                {
                    exitCrouch();
                }
            }

            if (isSliding)
            {
                // slide exit on jump
                if (this.speed <= crouchSpeed || Input.GetButtonDown("Jump") || Input.GetButtonUp("Crouch"))
                {
                    UnityEngine.Debug.Log("Conditions met to exit slide");
                    exitSlide();
                }

                if (isOnSlope)
                {
                    float speedX = (1f - currSlope.normal.y) * currSlope.normal.x * (1f - slideFriction);
                    float speedZ = (1f - currSlope.normal.y) * currSlope.normal.z * (1f - slideFriction);
                    speed += Math.Abs((speedZ + speedX) * 250f * Time.deltaTime);
                    charController.Move(currSlope.point - charController.transform.position);
                }
            }
        } else if (!isGrounded && !isOnSlope)
        {
            if (wasGrounded)
            {
                exitSprint();
                isCrouching = false;
                isSliding = false;
                charController.height = standingHeight;
                exitSlide();
            }

            // accelerate player in midair if they are moving forward
            if (speed < sprintSpeed && Input.GetAxisRaw("Vertical") > 0)
            {
                speed += airAccel * Time.deltaTime;
                if (speed >= sprintSpeed)
                {
                    speed = sprintSpeed;
                    isSprinting = true;
                }
            }
            if (speed > sprintSpeed)
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    speed -= 1 / (speed - sprintSpeed) * Time.deltaTime;
                } else if (Input.GetAxisRaw("Vertical") == 0)
                {
                    speed -= (speed - sprintSpeed) * Time.deltaTime;
                }
            }
        }

        //General gravity always applied to player when not wall running
        if (!isWallRunning)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0f;
        }
        //Jump Function using equation for gravity potential energy
        if (Input.GetButtonDown("Jump"))
        {
            bool inMidairCanJump = jumpCounter > 0 && !isGrounded;
            if (inMidairCanJump || isGrounded)
            {
                isJumping = true;
                //StartCoroutine(rotateCameraLeft());
                //UnityEngine.Debug.Log("jumping");
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                if (inMidairCanJump)
                {
                    jumpCounter--;
                }
            }

            //If player jumps while wallrunning, stop any wallrunning
            if (isWallRunning)
            {
                exitWallRun();
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }

        if (isOnSlope)
        {
            charController.Move(Vector3.down * slopeForce * Time.deltaTime);
        }

        // UnityEngine.Debug.Log("Speed = " + this.speed);

        move();

        charController.Move(velocity * Time.deltaTime);
    }

    void checkWall()
    {
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
            hitLeft.collider.gameObject.GetInstanceID() != lastWall)
        {
            if (!isWallRunning)
            {

                if (cameraRoutine != null)
                    StopCoroutine(cameraRoutine);
                cameraRoutine = StartCoroutine(rotateCamera(-20.0f));
            }
            enterWallRun(hitLeft);
        }
        else if (hitRight.collider != null &&
            checkHit(hitRight) &&
            hitRight.collider.gameObject.GetInstanceID() != lastWall)
        {

            if (!isWallRunning)
            {
                if (cameraRoutine != null)
                    StopCoroutine(cameraRoutine);
                cameraRoutine = StartCoroutine(rotateCamera(20.0f));
            }
            enterWallRun(hitRight);

        }
        else
        { //if there are no valid walls to either side of the player
            if (isWallRunning)
            {
                exitWallRun();
            }


            return;
        }
    }

    bool checkHit(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "Wall")
        {
            return true;
        }

        //Method suggested by footnotes
        //if (Vector3.Dot(hit.normal, Vector3.up) == 0) {
        //    print("WALL");
        //    return true;
        //}

        return false;
    }

    void enterWallRun(RaycastHit wall)
    {
        currentWall = wall.collider.gameObject.GetInstanceID();
        isWallRunning = true;

    }
    void exitWallRun()
    {
        resetJumps();
        isWallRunning = false;
        lastWall = currentWall;
        currentWall = null;
        if (cameraRoutine != null)
        {
            StopCoroutine(cameraRoutine);
            cameraRoutine = StartCoroutine(resetCamera());
        }
    }
    IEnumerator rotateCamera(float angle)
    {
        //print("ROTATION: " + camera.transform.eulerAngles.z);

        Vector3 currentRotation = camera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
        Vector3 currentRotate;
        //print("CURRENT: " + currentRotation + " TARGET: " + targetRotation);
        float duration = 0.3f;
        float time = 0.0f;

        while (time < duration)
        {

            currentRotate = Vector3.Lerp(currentRotation, targetRotation, time / duration);
            camera.transform.eulerAngles = currentRotate;
            //camera.transform.rotation = Quaternion.Euler(currentRotate);

            time += Time.deltaTime;

            yield return null;
        }

        camera.transform.eulerAngles = targetRotation;
    }
    IEnumerator resetCamera()
    {
        float targetZ = (camera.transform.eulerAngles.z > 180.0f) ? 360.0f : 0.0f;

        Vector3 currentRotation = camera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, targetZ);
        Vector3 currentRotate;
        float duration = 0.3f;
        float time = 0.0f;

        while (time < duration)
        {

            currentRotate = Vector3.Lerp(currentRotation, targetRotation, time / duration);
            camera.transform.rotation = Quaternion.Euler(currentRotate);

            time += Time.deltaTime;

            yield return null;
        }


        camera.transform.eulerAngles = targetRotation;
    }

    //checks if the player is on a slope
    private bool onSlope() {
        RaycastHit hit;

        //if the normal of the surface the player is standing on is not pointing up, then it is a slope
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10))
        {
            if (hit.normal != Vector3.up)
            {
                currSlope = hit;
                UnityEngine.Debug.Log("is on slope");
                return true;
            }
        }
        UnityEngine.Debug.Log("not on slope");
        return false;
    }

    private void move()
    {
        Vector3 movement;
        if (isSliding)
        {
            // restrict x axis movement if sliding
            movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed * slideHorizontalMoveMult, 0.0f, Math.Abs(Input.GetAxisRaw("Vertical") * speed));
        } else
        {
            movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0.0f, Input.GetAxisRaw("Vertical") * speed);
        } 

        if(movement.x != 0.0f && movement.z != 0.0f) {
            isMoving = true;
        }
        else {
            isMoving = false;
        }

        isMoving = (movement.x != 0.0f || movement.z != 0.0f) ? true : false;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        charController.Move(movement);
    }


    private void enterSprint()
    {
        isSprinting = true;
        playerLook.sprint(isSprinting);
        if (speed < sprintSpeed)
        {
            this.speed = sprintSpeed;
        }
    }

    private void exitSprint()
    {
        if (!isSliding && isGrounded)
        {
            isSprinting = false;
            playerLook.sprint(isSprinting);
            if (isCrouching)
            {
                this.speed = crouchSpeed;
            }
            else
            {
                this.speed = walkSpeed;
            }
        }
        isSprinting = false;
    }

    private void enterCrouch()
    {
        isCrouching = true;
        charController.height = crouchHeight;
        speed = crouchSpeed;
    }

    private void exitCrouch()
    {
        isCrouching = false;
        isSliding = false;
        charController.height = standingHeight;
        this.speed = walkSpeed;
    }

    private void enterSlide()
    {
        
        charController.height = crouchHeight;
        this.isSprinting = false;
        this.isSliding = true;
        slideRoutine = StartCoroutine(slideLerp());
    }

    private void exitSlide()
    {
        this.isSliding = false;
        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
        }

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {

            }
            else if (Input.GetButton("Crouch"))
            {
                playerLook.sprint(isSprinting);
                enterCrouch();
                this.speed = crouchSpeed;
            }
            else
            {
                playerLook.sprint(isSprinting);
                charController.height = standingHeight;
                if (speed < sprintSpeed)
                {
                    this.speed = sprintSpeed;
                }
            }
        }
    }

    private IEnumerator slideLerp()
    {
        float elapsedTime = 0f;
        // set change based on whether player is crouching
        while (elapsedTime < slideTime)
        {
            if (!isOnSlope && isGrounded)
            {
                this.speed = Mathf.Lerp(slideSpeed, 0.0f, elapsedTime / slideTime);
                if (this.speed < crouchSpeed)
                {
                    this.speed = crouchSpeed;
                    break;
                }
                elapsedTime += Time.deltaTime;
            } else
            {
                elapsedTime = 0f;
            }
            yield return null;
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Finish")
        {
            LevelEndScript levelEnd = hit.gameObject.GetComponent<LevelEndScript>();
            levelEnd.nextLevel();
        }
        else if (hit.gameObject.tag == "BouncePad")
        {
            BouncePad pad = hit.gameObject.GetComponent<BouncePad>();
            // bounce player off pad if pad is enabled
            if (pad.padEnabled)
            {
                resetGravity();
                resetJumps();
                charController.height = standingHeight;
                Vector3 padRotation = hit.gameObject.transform.eulerAngles;
                StartCoroutine(applyForce(pad.forceSpeed, Quaternion.Euler(padRotation.x, padRotation.y, padRotation.z) * pad.forceDir));
            }
        } else if (hit.gameObject.tag == "Enemy")
        {
            if (iTime <= 0)
            {
                Vector3 push = (charController.transform.position - hit.point).normalized;
                iTime = iTimeMax;
                StartCoroutine(applyForce(15f, push));
            }
        }

        if (isGrounded || isWallRunning)
        {
            MoveableObj moveableObj = hit.gameObject.GetComponent<MoveableObj>();
            if (moveableObj != null && moveableObj.getFinishedMove())
            {
                speed += moveableObj.getLastMove().magnitude;

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // UnityEngine.Debug.Log("hit trigger: " + other.gameObject.tag);
        if (other.gameObject.tag == "DeathPlane")
        {
            respawn();
        }
    }

    public void resetGravity()
    {
        this.velocity.y = -2.0f;
    }

    private void resetJumps()
    {
        this.jumpCounter = maxAirJumps;
    }

    private void respawn()
    {
        charController.enabled = false;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        charController.enabled = true;
        speed = walkSpeed;
    }

    private IEnumerator applyForce(float force, Vector3 direction)
    {
        float appliedForce = force;
        while ((appliedForce > 5f && !isGrounded && !isOnSlope && !isWallRunning && !wasGrounded) || (appliedForce > 5f && (isGrounded || isOnSlope || isWallRunning) && wasGrounded))
        {
            UnityEngine.Debug.Log("force = " + appliedForce);
            charController.Move(direction * appliedForce * Time.deltaTime);
            appliedForce -= appliedForce * Time.deltaTime;
            yield return null;
        }
    }
}

