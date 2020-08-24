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
    private bool isWallRunning, isJumping;
    private Animator animator;
    private float gravity = -9.81f; //default value of gravity in Unity
    Vector3 velocity; // Used for gravity



    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
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
        RaycastHit hitLeft, hitRight;
        Vector3 left = transform.InverseTransformPoint(Vector3.left);
        Vector3 right = transform.InverseTransformPoint(Vector3.right);

        float raycastLength = 10.0f;

        Physics.Raycast(transform.position, left, out hitLeft, raycastLength);

        UnityEngine.Debug.DrawRay(transform.position, left, Color.red);

        Physics.Raycast(transform.position, right, out hitRight, raycastLength);
        UnityEngine.Debug.DrawRay(transform.position, right, Color.red);

        if (hitLeft.collider != null) {
            UnityEngine.Debug.Log("Wall to the Left");
        }
        if (hitRight.collider != null) {
            UnityEngine.Debug.Log("Wall to the Right");
        }

    }

}

