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
    private bool isWallRunning, isJumping, isCrouching;
    private Animator animator;
    private float gravity = -9.81f; //default value of gravity in Unity
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
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0.0f, Input.GetAxisRaw("Vertical") * speed);

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        charController.Move(movement);

        //Jump Function using equation for gravity potential energy
        if(Input.GetButtonDown("Jump")) {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            enterCrouchSlide();
        }

        if (Input.GetButtonUp("Crouch"))
        {
            exitCrouchSlide();
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

    private void Wallrun() {
        

    }

    private void enterCrouchSlide()
    {
        isCrouching = true;
        charController.height = 1.5f;
        Vector3 crouchPos = new Vector3(playerLook.camera.position.x, playerLook.camera.position.y - 0.25f, playerLook.camera.position.z);
        playerLook.camera.position = Vector3.Lerp(playerLook.camera.position, crouchPos, 0f);
    }

    private void exitCrouchSlide()
    {
        isCrouching = false;
        charController.height = 2.0f;
        Vector3 crouchPos = new Vector3(playerLook.camera.position.x, playerLook.camera.position.y + 0.25f, playerLook.camera.position.z);
        playerLook.camera.position = Vector3.Lerp(playerLook.camera.position, crouchPos, 0f);
    }
}

