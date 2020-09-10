using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float force, smooth;
    private Quaternion originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion tmpX = Quaternion.AngleAxis(-force * mouseX, Vector3.up);
        Quaternion tmpY = Quaternion.AngleAxis(force * mouseY, Vector3.right);
        Quaternion targetRotation = originalRotation * tmpX * tmpY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
