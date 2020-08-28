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
    public Transform groundCheck;
    private bool isGrounded;
    private CharacterController charController;
    private PlayerLook playerLook;
    private bool isWallRunning, isJumping, isCrouching, isSprinting;
    private Animator animator;
    private float gravity = -9.81f; //default value of gravity in Unity
    private float walkSpeed = 3f;
    private float sprintSpeed = 7f;
    private float crouchTime = 2f;
    private float crouchHeight = 1.5f;
    private float standingHeight = 2.0f;
    private Coroutine crouchHeightChange = null;
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
        movement();

    }

    //Jumping and gravity obtained from: https://youtu.be/_QajrabyTJc
    private void movement() {

        Wallrun();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        
        if (Input.GetButtonDown("Sprint"))
        {
            enterSprint();
        }
        if (Input.GetButtonUp("Sprint")){
            exitSprint();
        }

        move();

        //Jump Function using equation for gravity potential energy
        if(Input.GetButtonDown("Jump")) {
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

        //General gravity always applied to player
        velocity.y += gravity * Time.deltaTime;

        if (onSlope()) {
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

    private void Wallrun() {
        

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

