using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float rotationSpeed;
    public Transform camera;
    private float minVertical = -45.0f;
    private float maxVertical = 45.0f;
    private float rotationY = 0;
    private float rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);

        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;


        transform.eulerAngles = new Vector3(0, rotationY, 0);
        camera.transform.eulerAngles = new Vector3(rotationX, rotationY, camera.transform.eulerAngles.z);
    }

}
