using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public float force, smooth;
    public Camera camera;
    public Transform gunEnd;
    public Texture2D crosshair;
    private Quaternion originalRotation;
    // Start is called before the first frame update
    void Start() {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update() {
        weaponSway();
        if (Input.GetButtonDown("Fire1")) {
            shoot();
        }

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
            }
        }
        // Physics.Raycast()
    }

    void weaponSway() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion tmpX = Quaternion.AngleAxis(-force * mouseX, Vector3.up);
        Quaternion tmpY = Quaternion.AngleAxis(force * mouseY, Vector3.right);
        Quaternion targetRotation = originalRotation * tmpX * tmpY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    private void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshair.width / 2), 
             (Screen.height / 2) - crosshair.height / 2, crosshair.width, crosshair.height), crosshair);
    }
}
