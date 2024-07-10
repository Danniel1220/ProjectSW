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
    [SerializeField] private float cameraMaxYClamp = 4f;
    [SerializeField] private float cameraMinYClamp = -4f;

    [SerializeField] private float cameraYMultiplier = 0.05f;
    [SerializeField] private float cameraXMultiplier = 100f;

    [SerializeField] private float zoomMinDistance = 4;
    [SerializeField] private float zoomMaxDistance = 12;
    [SerializeField] private float zoomCurrentMultiplier = 1f;
    [SerializeField] private float zoomSmoothingFactor = 2f;

    [SerializeField] private float zoomDelta;
    [SerializeField] private float targetZoom = 4f;

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
                /*
                cameraAnchorLookPoint.position = new Vector3(
                    cameraAnchorLookPoint.position.x,
                    Mathf.Clamp(cameraAnchorLookPoint.position.y + (delta.y * cameraYMultiplier), 
                        cameraAnchor.position.y + cameraMinYClamp, 
                        cameraAnchor.position.y + cameraMaxYClamp), // camera is clamped on the (local) Y axis
                    cameraAnchorLookPoint.position.z);
                */

                cameraAnchorLookPoint.position += cameraAnchorLookPoint.TransformDirection(Vector3.up) * delta.y * cameraYMultiplier;

            }
            // if the mouse moved on x axis on screen, rotate the anchor look point around the anchor
            if (delta.x != 0)
            {
                //cameraAnchorLookPoint.RotateAround(cameraAnchor.position, Vector3.up, delta.x * Time.deltaTime * cameraXMultiplier);
                //defaultSpaceshipLookPoint.RotateAround(cameraAnchor.position, Vector3.up, delta.x * Time.deltaTime * cameraXMultiplier);

                shipTransform.RotateAround(shipTransform.position, Vector3.up, delta.x * cameraXMultiplier * Time.deltaTime);
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
