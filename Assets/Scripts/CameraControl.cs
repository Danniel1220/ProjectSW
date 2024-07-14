using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;
    private Transform shipTransform;

    private Transform cameraAnchor;
    private Transform cameraAnchorLookPoint;

    private Vector3 lastMousePosition;
    private float cameraMaxYClamp = 4f;

    private float cameraYMultiplier = 0.05f;
    private float cameraXMultiplier = 100f;

    private float zoomMinDistance = 4;
    private float zoomMaxDistance = 12;
    private float zoomCurrentMultiplier = 1f;
    private float zoomSmoothingFactor = 2f;

    private float zoomDelta;
    private float targetZoom = 4f;

    // Start is called before the first frame update
    void Start()
    {
        cinemachine3RdPersonFollow = GetComponentInChildren<Cinemachine3rdPersonFollow>();
        shipTransform = GameObject.Find("Spaceship").GetComponent<Transform>();
        cameraAnchor = GameObject.Find("Spaceship").transform.Find("CameraAnchor").GetComponent<Transform>();
        cameraAnchorLookPoint = GameObject.Find("Spaceship").transform.Find("CameraAnchorLookPoint").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        handleCameraRotation();
        handleCameraZoom();
    }
    void LateUpdate()
    {
        cameraAnchor.LookAt(cameraAnchorLookPoint.position);
    }

    public void handleCameraRotation()
    {
        // if right mouse button is pressed down, store the position where the click happened
        if (Input.GetMouseButtonDown(1)) lastMousePosition = Input.mousePosition;

        // if right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            // calculate the distance it moves on each axis since it was first pressed down
            Vector3 delta = (Input.mousePosition - lastMousePosition);
            lastMousePosition = Input.mousePosition;

            // if the mouse moved on y axis on screen move the anchor look point up or down
            if (delta.y != 0)
            {
                cameraAnchorLookPoint.position += cameraAnchorLookPoint.TransformDirection(Vector3.up) * delta.y * cameraYMultiplier;
                cameraAnchorLookPoint.position = new Vector3(
                    cameraAnchorLookPoint.position.x, 
                    Mathf.Clamp(cameraAnchorLookPoint.position.y, shipTransform.position.y - cameraMaxYClamp, shipTransform.position.y + cameraMaxYClamp), 
                    cameraAnchorLookPoint.position.z);
                }
            // if the mouse moved on x axis on screen, rotate the anchor look point around the player
            if (delta.x != 0)
            {
                //cameraAnchorLookPoint.RotateAround(shipTransform.position, cameraAnchorLookPoint.up, delta.x * cameraXMultiplier * Time.deltaTime);
                GameManagers.spaceshipMovementController.applyCameraRotation(delta.x);
            }

        }
    }

    public void handleCameraZoom()
    {
        zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomCurrentMultiplier;
        targetZoom -= zoomDelta; // subtracting value because mwheel inverse logic for zoom is prefered
        targetZoom = Mathf.Clamp(targetZoom, zoomMinDistance, zoomMaxDistance); // limiting zoom to be between min/max values
        cinemachine3RdPersonFollow.CameraDistance = Mathf.Lerp(cinemachine3RdPersonFollow.CameraDistance, targetZoom, Time.deltaTime * zoomSmoothingFactor); // smoothing zoom
    }
}
