using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovementController : MonoBehaviour
{
    Transform shipTransform;
    Transform shipModelTransform;
    Transform defaultSpaceshipLookPoint;
    Transform cursorWorldSpacePosition;
    GameObject gravityBoundGameObject;

    float forwardMovement;
    float backwardMovement;
    float rightMovement;
    float leftMovement;
    float upMovement;
    float downMovement;

    float movementSpeed = 5f;

    float minLookAtDistance = 2f;
    float maxLookAtDistance = 12f;
    float lookAtRotationSpeed = 10f;

    float planetSize = 100f;
    float heightAbovePlanet = 2f;

    void Start()
    {
        shipTransform = GameObject.Find("Spaceship").GetComponent<Transform>();
        shipModelTransform = GetComponent<Transform>().Find("Model");
        cursorWorldSpacePosition = GameManagers.cursorLocationInWorldSpace.transform;
        defaultSpaceshipLookPoint = GetComponent<Transform>().Find("DefaultSpaceshipLookPoint");
        gravityBoundGameObject = GameObject.Find("Planet");

    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W) == true)
        {
            forwardMovement = 1f;
        }
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
        orientation();
    }

    void movement()
    {
        // W
        shipTransform.RotateAround(gravityBoundGameObject.transform.position, shipTransform.forward, forwardMovement * movementSpeed * Time.deltaTime * 5);
        //shipTransform.position += transform.forward * forwardMovement * movementSpeed * Time.deltaTime;

        // S
        shipTransform.position -= transform.forward * backwardMovement *  movementSpeed * Time.deltaTime;


        // A
        shipTransform.position += transform.right * rightMovement * movementSpeed * Time.deltaTime;


        // D
        shipTransform.position -= transform.right * leftMovement * movementSpeed * Time.deltaTime;


        // Space
        //shipTransform.position += transform.up * upMovement * movementSpeed * Time.deltaTime;
        // going up means moving away from the body we are gravitationally bound to
        shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, -movementSpeed * upMovement * Time.deltaTime);

        // Left Shift
        //shipTransform.position -= transform.up * downMovement * movementSpeed * Time.deltaTime;
        // going down means moving towards the body we are gravitationally bound to
        shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, movementSpeed * downMovement * Time.deltaTime);

    }

    void orientation()
    {
        float shipCursorDistance = Vector3.Distance(shipTransform.position, cursorWorldSpacePosition.position);
        Quaternion targetRotation;

        if (shipCursorDistance > minLookAtDistance && shipCursorDistance < maxLookAtDistance) targetRotation = Quaternion.LookRotation(cursorWorldSpacePosition.position - shipModelTransform.position);
        else targetRotation = Quaternion.LookRotation(defaultSpaceshipLookPoint.position - shipModelTransform.position);

        shipModelTransform.rotation = Quaternion.Slerp(shipModelTransform.rotation, targetRotation, lookAtRotationSpeed * Time.deltaTime);
    }
}
