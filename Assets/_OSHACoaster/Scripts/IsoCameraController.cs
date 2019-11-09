using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCameraController : MonoBehaviour
{

    public Camera targetCamera;

    public float cameraSpeed = 1.0f;
    public float zoomSpeed = 500.0f;
    public float pitchSpeed = 15.0f;

    public float zoomMin = 30.0f;
    public float zoomMax = 100.0f;

    public float controlPitch = 30.0f;
    public float pitchMin = 10.0f;
    public float pitchMax = 80.0f;

    private Vector2 savedCursorPosition;

    void Update()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Fire2"))
        {
            float pitchDelta = Input.GetAxisRaw("Mouse Y");
            controlPitch = Mathf.Clamp(controlPitch + pitchDelta * pitchSpeed * Time.deltaTime, pitchMin, pitchMax);
        }

        // TODO: save and restore mouse position
        if(Input.GetButtonDown("Fire2"))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            //savedCursorPosition = Input.mousePosition;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Input.mousePosition = savedCursorPosition;
        }

        const float yawConst = 45.0f;
        Quaternion yawQuaternion = Quaternion.AngleAxis(yawConst, Vector3.up);
        Vector3 prePitchFwd = yawQuaternion * Vector3.forward;
        Debug.DrawRay(transform.position, prePitchFwd * 10.0f, Color.blue, 0.0f);
        Vector3 finalPitchAxis = -Vector3.Cross(prePitchFwd, Vector3.up);
        Debug.DrawRay(transform.position, finalPitchAxis * 10.0f, Color.red, 0.0f);
        transform.rotation = Quaternion.AngleAxis(controlPitch, finalPitchAxis) * yawQuaternion;

        float zMove = -Input.GetAxisRaw("Mouse ScrollWheel");

        Vector3 pMove = (targetCamera.transform.right * hMove +
                         Vector3.Cross(targetCamera.transform.right, Vector3.up) * vMove).normalized * cameraSpeed * Time.deltaTime;

        targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView + zMove * zoomSpeed * Time.deltaTime, zoomMin, zoomMax);
        transform.Translate(pMove, Space.World);
    }

    void Reset()
    {
        targetCamera = GetComponent<Camera>();
    }
}
