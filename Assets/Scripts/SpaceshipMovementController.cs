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
    bool[] inputValues = new bool[6];

    [SerializeField] float movementSpeed = 20f;

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
        getKeyboardInput();
        handleMovement();
    }

    private void getKeyboardInput()
    {
        if (Input.GetKey(KeyCode.W) == true) inputValues[0] = true;
        else inputValues[0] = false;

        if (Input.GetKey(KeyCode.A) == true) inputValues[1] = true;
        else inputValues[1] = false;

        if (Input.GetKey(KeyCode.S) == true) inputValues[2] = true;
        else inputValues[2] = false;

        if (Input.GetKey(KeyCode.D) == true) inputValues[3] = true;
        else inputValues[3] = false;

        if (Input.GetKey(KeyCode.Space) == true) inputValues[4] = true;
        else inputValues[4] = false;

        if (Input.GetKey(KeyCode.LeftShift) == true) inputValues[5] = true;
        else inputValues[5] = false;
    }

    private void handleMovement()
    {
        // W
        if (inputValues[0])
        {
            shipTransform.position += transform.forward * movementSpeed * Time.deltaTime;
        }
        // A
        if (inputValues[1])
        {
            shipTransform.position -= transform.right * movementSpeed * Time.deltaTime;
        }
        // S
        if (inputValues[2])
        {
            shipTransform.position -= transform.forward *  movementSpeed * Time.deltaTime;
        }
        // D
        if (inputValues[3])
        {
            shipTransform.position += transform.right * movementSpeed * Time.deltaTime;
        }
        // Space
        if (inputValues[4])
        {
            // going up means moving away from the body we are gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, -movementSpeed * Time.deltaTime);
            heightAbovePlanet += movementSpeed * Time.deltaTime; 
        }
        // Left Shift
        if (inputValues[5])
        {
            // going down means moving towards the body we are gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, movementSpeed * Time.deltaTime);
            heightAbovePlanet -= movementSpeed * Time.deltaTime;
        }

        // gets the normalized direction vector from center of the planet to the ship after it moved forward
        // and then sets the position of the ship again to preserve distance from planet
        Vector3 spaceShipDirection = (shipTransform.position - gravityBoundGameObject.transform.position).normalized;
        shipTransform.position = gravityBoundGameObject.transform.position + spaceShipDirection * (planetSize / 2 + heightAbovePlanet);
        shipTransform.up = spaceShipDirection;
    }
}
