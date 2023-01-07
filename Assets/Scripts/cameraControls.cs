using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [Header("----- Camera Sensitivity -----")]
    [Range(100, 1000)][SerializeField] int sensHor;
    [Range(100, 1000)][SerializeField] int sensVer;

    [Header("----- Vertical Lock Degrees -----")]
    [Range(-90, -50)][SerializeField] int lockVerMin;
    [Range(90, 50)][SerializeField] int lockVerMax;

    [Header("----- Misc. -----")]
    [SerializeField] bool invertX;

    float xRotation;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;

        if (invertX)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        // clamp the camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        // rotate the camera on its X-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate the player on its Y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
