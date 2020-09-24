using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public float force, smooth;
    public float idleCounter, movementCounter;
    public Camera camera;
    public Transform gunEnd;
    public Texture2D crosshair;
    private Quaternion originalRotation;
    private PlayerMovement playerMovement;
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start() {
        originalRotation = transform.rotation;
        playerMovement = transform.root.gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {

        //if (GameManager.Instance.isPaused)
        //    return;

        if (Input.GetButtonDown("Fire1")) {
            shoot();
        }

    }
    private void FixedUpdate() {
        lookSway();
        movementSway();
    }

    void shoot() {
        Vector3 point = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
        Ray ray = camera.ScreenPointToRay(point);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            print(hit.collider.tag);
            switch (hit.collider.tag) {
                case "Enemy":
                    Destroy(hit.collider.gameObject);
                    break;
                case "Switch":
                    hit.collider.gameObject.GetComponent<SwitchScript>().enable();
                    break;
                case "BouncePad":
                    BouncePad pad = hit.collider.gameObject.GetComponent<BouncePad>();
                    if (!pad.padEnabled)
                    {
                        pad.padEnabled = true;
                        hit.collider.gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    }
                    break;
            }
        }
        // Physics.Raycast()
    }

    //Gun sway when moving the camera
    void lookSway() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion tmpX = Quaternion.AngleAxis(-force * mouseX, Vector3.up);
        Quaternion tmpY = Quaternion.AngleAxis(force * mouseY, Vector3.right);
        Quaternion targetRotation = originalRotation * tmpX * tmpY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    private void movementSway() {
        //TODO: Clean up this if else tree
        if (playerMovement.isWallRunning) {
            /*Subtract 1 from x as Mathf.PingPong starts at 0, and we need
            values from -1 to 1 for parabola */
            wallRunSway((Mathf.PingPong(Time.time * 6f, 4f) - 2), 0.1f, 0.01f);

            //Lerp to target position so that weapon does not 'snap' when changing between movement types
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8);
        }
        else if (playerMovement.isSprinting) {
            weaponSway(idleCounter, 0.08f, 0.08f);
            idleCounter += Time.deltaTime * 8;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 4);
        }
        else if (playerMovement.isMoving) {
            weaponSway(idleCounter, 0.05f, 0.05f);
            idleCounter += Time.deltaTime * 2;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 4);
        }
        else {
            weaponSway(idleCounter, 0.01f, 0.01f);
            idleCounter += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 4);
        }
    }

    private void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshair.width / 2), 
             (Screen.height / 2) - crosshair.height / 2, crosshair.width, crosshair.height), crosshair);
    }

    //Sets target position to lerp to for weapon sway during movement
    private void weaponSway(float x, float xSwayIntensity, float ySwayIntensity) {

        //in a 2D space, Cos gives x coordinate and Sin gives y coordinate
        targetPosition = new Vector3(Mathf.Cos(x) * xSwayIntensity, Mathf.Sin(2 * x) * ySwayIntensity, transform.localPosition.z);
    }

    private void wallRunSway(float x, float xSwayIntensity, float ySwayIntensity) {
        targetPosition = new Vector3((x)  * xSwayIntensity, -Mathf.Pow(x,2)  * ySwayIntensity, transform.localPosition.z);
    }
}
