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

    bool[] inputValues = new bool[6];

    float movementSpeed = 20f;

    float yawAngle = 0f;
    float pitchAngle = 25f;
    float rollAngle = 0f;

    float yawTurnSpeed = 0f;
    float pitchTurnSpeed = 12f;
    float rollTurnSpeed = 0f;

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
        float shipTransformCurrentRotationX = shipTransform.rotation.eulerAngles.x;

        // forwards-backwards and up-down movement

        // assume the player does not move at all at first
        float movementDelta = 0;
        float altitudeDelta = 0;
        float distanceFromGravityBoundObject = Vector3.Distance(gravityBoundGameObject.transform.position, shipTransform.position);

        // W
        if (inputValues[0])
        {
            shipTransform.position += transform.forward * movementSpeed * Time.deltaTime;
            movementDelta = movementSpeed * Time.deltaTime;
        }
        // S
        else if (inputValues[2])
        {
            shipTransform.position -= transform.forward * movementSpeed * Time.deltaTime;
            movementDelta = movementSpeed * Time.deltaTime;
        }
        // Space + Left Shift
        if (inputValues[4] && inputValues[5])
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                    shipModelTransform.rotation,
                    shipTransform.rotation,
                    pitchTurnSpeed * Time.deltaTime);
        }
        // Space
        else if (inputValues[4])
        {
            // going up means moving away from the body the player is gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, -movementSpeed * Time.deltaTime);
            heightAbovePlanet += movementSpeed * Time.deltaTime;
            altitudeDelta = movementSpeed * Time.deltaTime;
            shipModelTransform.rotation = Quaternion.Slerp(
                shipModelTransform.rotation, 
                shipTransform.rotation * Quaternion.Euler(-pitchAngle, 0, 0), 
                pitchTurnSpeed * Time.deltaTime);
        }
        // Left Shift
        else if (inputValues[5])
        {
            // going down means moving towards the body the player is gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, movementSpeed * Time.deltaTime);
            heightAbovePlanet -= movementSpeed * Time.deltaTime;
            altitudeDelta = -movementSpeed * Time.deltaTime;
            shipModelTransform.rotation = Quaternion.Slerp(
                shipModelTransform.rotation,
                shipTransform.rotation * Quaternion.Euler(pitchAngle, 0, 0),
                pitchTurnSpeed * Time.deltaTime);
        }
        else
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                    shipModelTransform.rotation,
                    shipTransform.rotation,
                    pitchTurnSpeed * Time.deltaTime);
        }


        float distanceFromGravityBoundObject2 = Vector3.Distance(gravityBoundGameObject.transform.position, shipTransform.position);

        // gets the normalized direction vector from center of the planet to the ship after it moved forward
        // and then sets the position of the ship again to preserve distance from planet
        Vector3 spaceShipDirection = (shipTransform.position - gravityBoundGameObject.transform.position).normalized;
        shipTransform.position = gravityBoundGameObject.transform.position + spaceShipDirection * (planetSize / 2 + heightAbovePlanet);
        //shipTransform.up = spaceShipDirection;

        // this represents the difference in altitude between the moment where the player is stationary and the moment after the movement is applied
        float realAltitudeDelta = distanceFromGravityBoundObject2 - distanceFromGravityBoundObject - altitudeDelta;
        // pythagorean theorem to find the distance traveled in the current frame
        float diagonal = Mathf.Sqrt(realAltitudeDelta * realAltitudeDelta + movementDelta * movementDelta);
        // the angle at which the ship should turn on the local X axis after the movement was applied
        // in an ABC right scalence triangle where ABC is the right angle, AB is the long side (movementDelta), BC (realAltitudeDelta) is the short side
        // this would be angle BAC
        float angle = Mathf.Asin(realAltitudeDelta / diagonal) * (180 / Mathf.PI);
        // fallback to 0 if the calculation fails (this happens when no movement is done)
        // also if the angle is 90 or -90 degrees it means the angle float underflowed because the realAltitudeDelta has a stupidly small value in it
        if (float.IsNaN(angle) || !(angle > -90 && angle < 90)) angle = 0;
        shipTransform.Rotate(angle, 0, 0);




        // left-right movement

        // A
        if (inputValues[1])
        {
            shipTransform.Rotate(new Vector3(0, -movementSpeed * Time.deltaTime, 0), Space.Self);
        }

        // D
        if (inputValues[3])
        {
            shipTransform.Rotate(new Vector3(0, movementSpeed * Time.deltaTime, 0), Space.Self);
        }
    }
}
