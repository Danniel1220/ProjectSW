using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovementController : MonoBehaviour
{
    Transform shipTransform;
    Transform shipModelTransform;
    Transform cameraAnchorLookPointTransform;
    Transform defaultSpaceshipLookPoint;
    Transform cursorWorldSpacePosition;

    [SerializeField] float forwardMovement;
    [SerializeField] float backwardMovement;
    [SerializeField] float rightMovement;
    [SerializeField] float leftMovement;
    [SerializeField] float upMovement;
    [SerializeField] float downMovement;

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float yawSpeed = 50f;

    [SerializeField] float minDistance = 2f;
    [SerializeField] float lookAtRotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        shipTransform = GetComponent<Transform>();
        shipModelTransform = GetComponent<Transform>().Find("Model");
        cursorWorldSpacePosition = GameManagers.cursorLocationInWorldSpace.transform;
        cameraAnchorLookPointTransform = GetComponent<Transform>().Find("CameraAnchorLookPoint");
        defaultSpaceshipLookPoint = GetComponent<Transform>().Find("DefaultSpaceshipLookPoint");

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) == true) forwardMovement = 1f;
        else forwardMovement = 0f;

        if(Input.GetKey(KeyCode.S) == true) backwardMovement = 1f;
        else backwardMovement = 0f;

        if (Input.GetKey(KeyCode.A) == true) leftMovement = 1f;
        else leftMovement = 0f;

        if (Input.GetKey(KeyCode.D) == true) rightMovement = 1f;
        else rightMovement = 0f;

        if (Input.GetKey(KeyCode.Space) == true) upMovement = 1f;
        else upMovement = 0f;

        if (Input.GetKey(KeyCode.LeftShift) == true) downMovement = 1f;
        else downMovement = 0f;

        movement();
        lookAt();
    }

    void movement()
    {
        // W
        shipTransform.position += transform.forward * forwardMovement *  movementSpeed * Time.deltaTime;

        // S
        shipTransform.position -= transform.forward * backwardMovement *  movementSpeed * Time.deltaTime;

        // A
        shipTransform.position += transform.right * rightMovement * movementSpeed * Time.deltaTime;
        shipTransform.Rotate(0, rightMovement * yawSpeed * Time.deltaTime, 0);

        // D
        shipTransform.position -= transform.right * leftMovement * movementSpeed * Time.deltaTime;
        shipTransform.Rotate(0, -(leftMovement * yawSpeed * Time.deltaTime), 0);

        // Space
        shipTransform.position += transform.up * upMovement * movementSpeed * Time.deltaTime;

        // Left Shift
        shipTransform.position -= transform.up * downMovement * movementSpeed * Time.deltaTime;
    }

    void lookAt()
    {
        float distance = Vector3.Distance(shipTransform.position, cursorWorldSpacePosition.position);
        Debug.Log(distance);
        Quaternion targetRotation;

        if (distance > minDistance) targetRotation = Quaternion.LookRotation(cursorWorldSpacePosition.position - shipModelTransform.position);
        else targetRotation = Quaternion.LookRotation(defaultSpaceshipLookPoint.position - shipModelTransform.position);

        shipModelTransform.rotation = Quaternion.Slerp(shipModelTransform.rotation, targetRotation, lookAtRotationSpeed * Time.deltaTime);
    }
}
