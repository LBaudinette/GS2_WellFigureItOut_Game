using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float rotationSpeed;
    public Transform cameraTranform;
    public Camera camera, gunCamera;
    private float minVertical = -90.0f;
    private float maxVertical = 90.0f;
    private float rotationY = 0;
    private float rotationX = 0;
    private float defaultFoV = 80f;
    private float sprintFoV = 100f;
    private float sprintFoVChangeTime = 1f;
    private Coroutine sprintFoVChange = null;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gunCamera = camera.transform.Find("Gun Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);

        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;


        transform.eulerAngles = new Vector3(0, rotationY, 0);
        camera.transform.eulerAngles = new Vector3(rotationX, rotationY, camera.transform.eulerAngles.z);
    }

    public void sprint(bool isSprinting)
    {
        if (sprintFoVChange != null)
        {
            StopCoroutine(sprintFoVChange);
        }
        sprintFoVChange = StartCoroutine(sprintFoVLerp(isSprinting));
    }

    public IEnumerator sprintFoVLerp(bool isSprinting)
    {
        float elapsedTime = 0;
        // set endpoint of change based on whether player is sprinting
        float endFoV = isSprinting ? sprintFoV : defaultFoV;

        while (elapsedTime < sprintFoVChangeTime)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, endFoV, elapsedTime / sprintFoVChangeTime);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, endFoV, elapsedTime / sprintFoVChangeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }
}
