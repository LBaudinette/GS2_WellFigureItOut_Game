using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{

    private CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        Wallrun();
    }


    void Wallrun() {
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

        if (hitLeft.collider != null) {
            UnityEngine.Debug.Log("Wall to the Left");
        }
        if (hitRight.collider != null) {
            UnityEngine.Debug.Log("Wall to the Right");
        }
    }
}

