using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public float force, smooth;                 //the intensity of weapon look sway and smoothing for LERP
    public float idleCounter, movementCounter;
    public Camera camera;
    public Transform gunEnd;                    //transform for the gun end found as a child of the gun object
    public Texture2D crosshair;                 //texture for the crosshair that this script draws
    private Quaternion originalRotation;        //the original rotation for the gun
    private PlayerMovement playerMovement;      //reference to the player movement script to check for movement states 
    private Vector3 targetPosition;             //used for weapon sway to lerp to target position
    private LineRenderer laser;      
    
    private float nextShoot;                    //the time for when the player can shoot again
    private float rateOfFire = 0.5f;              //the delay between when the player can shoot multiple times
    private float weaponRange = 50.0f;          //range of the weapon if nothing is hit
    private float laserTimer = 0.1f;            //how long it takes for the laser to disappear
    private float origStartWidth,origEndWidth;  //the original starting and ending widths for the line renderer

    public float rotationRecoverSpeed;          //How quickly the weapon rotation recovers from recoil
    public float positionRecoverSpeed;          //How quickly the weapon position recovers from recoil
    public float positionRecoilSpeed;           //How fast the gun kick backs for recoil
    public float rotationRecoilSpeed;           //How fast the gun rotates for recoil
    private Vector3 targetRecoilRotation;        //the target rotation for recoil that is converted to a quaternion 

    private Vector3 rotationRecoil = new Vector3(100, 50,0);             //Intensity for recoil rotation
    private Vector3 kickbackRecoil = new Vector3(5f, 4f,0);             //Intensity for recoil kick back

    private Vector3 rotationalRecoil;           //Recoil relating to the rotation of the weapon
    private Vector3 positionalRecoil;           //Recoil relation to the position of the weapon
    
    

    void Start() {
        originalRotation = transform.rotation;
        playerMovement = transform.root.gameObject.GetComponent<PlayerMovement>();
        laser = GetComponent<LineRenderer>();
        origStartWidth = laser.startWidth;
        origEndWidth = laser.endWidth;
        
    }

    void Update() {
        
        
        if (Input.GetButtonDown("Fire1") && Time.time > nextShoot) {
            /*add the rate of fire to the current time to find what time the
            player can shoot again*/
            nextShoot = Time.time + rateOfFire;
            shoot();
        }

    }

    private void FixedUpdate() {
        //rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationRecoverSpeed * Time.deltaTime);
        //positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionRecoverSpeed * Time.deltaTime);

        //transform.localPosition = Vector3.Slerp(transform.localPosition, positionalRecoil, positionRecoilSpeed * Time.fixedDeltaTime);
        //targetRecoilRotation = Vector3.Slerp(targetRecoilRotation, rotationalRecoil, rotationRecoilSpeed * Time.fixedDeltaTime);
        //transform.localRotation = Quaternion.Euler(targetRecoilRotation);

        lookSway();
        movementSway();
    }

    private void LateUpdate() {
        
    }

    void shoot() {
        
        //Find the point in the middle of the screen
        Vector3 point = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);

        //convert that point into a ray going from the camera to the point
        Ray ray = camera.ScreenPointToRay(point);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        //Handle the raycast hit
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            print(hit.collider.gameObject.name);
            switch (hit.collider.tag) {
                case "Enemy":
                    Destroy(hit.collider.gameObject);
                    break;
                case "Switch":
                    hit.collider.gameObject.GetComponent<SwitchScript>().enable();
                    break;
                case "MoveableObjSwitch":
                    hit.collider.gameObject.GetComponent<MoveableObjSwitch>().enable();
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
            createLaser(hit.point);
        } 
        else { //if the player didn't hit anything, then only draw a laser to a certain distance
            createLaser(ray.origin + (camera.transform.forward * weaponRange));

        }

        //recoil();
    }
    void createLaser(Vector3 endPoint) {
        laser.enabled = true;
        laser.SetPosition(0, gunEnd.position);
        laser.SetPosition(1, endPoint);
        //laser.startWidth = 0.1f;
        //laser.endWidth = 0.5f;
        StartCoroutine(fadeLasers());
    }

    IEnumerator fadeLasers() {
        //gradually reduce laser width
        while(laserTimer <= rateOfFire) {
            laserTimer += Time.deltaTime;
            laser.startWidth = Mathf.Lerp(laser.startWidth, 0.0f, laserTimer / rateOfFire);
            laser.endWidth = Mathf.Lerp(laser.endWidth, 0.0f, laserTimer / rateOfFire);
            
            yield return null;
        }

        //reset values once laser has disappeared
        laserTimer = 0.0f;
        laser.enabled = false;
        laser.startWidth = origStartWidth;
        laser.endWidth = origEndWidth;
    }

    void recoil() {
        rotationalRecoil += new Vector3(-rotationRecoil.x, rotationRecoil.y, rotationRecoil.z);
        positionalRecoil += new Vector3( kickbackRecoil.x, kickbackRecoil.y, kickbackRecoil.z);
    }

    //Gun sway when moving the camera
    void lookSway() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Create a x and y rotation for the look sway depending on what way the player looked
        Quaternion tmpX = Quaternion.AngleAxis(-force * mouseX, Vector3.up);
        Quaternion tmpY = Quaternion.AngleAxis(force * mouseY, Vector3.right);

        //multiplying Quaternions adds the angles together
        Quaternion targetRotation = originalRotation * tmpX * tmpY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);

    }



    private void movementSway() {

        //TODO: Clean up this if else tree
        if (playerMovement.isWallRunning) {
            /*Subtract 2 from x as Mathf.PingPong starts at 0, and we need
            values from -2 to 2 for parabola */
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
        //Use a porabola for wall run sway
        targetPosition = new Vector3((x)  * xSwayIntensity, -Mathf.Pow(x,2)  * ySwayIntensity, transform.localPosition.z);
    }
}
