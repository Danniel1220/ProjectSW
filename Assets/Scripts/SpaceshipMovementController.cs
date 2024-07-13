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
    float turnSpeed = 40f;

    float yawAngle = 0f;
    float pitchAngle = 60f;
    float rollAngle = 30f;

    float yawTurnSpeed = 0f;
    float pitchTurnSpeed = 8f;
    float rollTurnSpeed = 12f;
    float fallbackTurnSpeed = 20f;

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
        // default ship target rotation is equal to zero for each axis
        Quaternion shipTargetPitch = Quaternion.identity;
        Quaternion shipTargetRoll = Quaternion.identity;

        // assume the player does not move at all at first
        float movementDelta = 0;
        float altitudeDelta = 0;
        float distanceFromGravityBoundObject = Vector3.Distance(gravityBoundGameObject.transform.position, shipTransform.position);


        // forwards-backwards and up-down movement

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
            movementDelta = -movementSpeed * Time.deltaTime;
        }

        // Space + Left Shift
        if (inputValues[4] && inputValues[5])
        {
            // do nothing basically
        }
        // Space
        else if (inputValues[4])
        {
            // going up means moving away from the body the player is gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, -movementSpeed * Time.deltaTime);
            heightAbovePlanet += movementSpeed * Time.deltaTime;
            altitudeDelta = movementSpeed * Time.deltaTime;
            // pitch angle
            shipTargetPitch *= Quaternion.Euler(-pitchAngle, 0, 0);

        }
        // Left Shift
        else if (inputValues[5])
        {
            // going down means moving towards the body the player is gravitationally bound to
            shipTransform.position = Vector3.MoveTowards(shipTransform.position, gravityBoundGameObject.transform.position, movementSpeed * Time.deltaTime);
            heightAbovePlanet -= movementSpeed * Time.deltaTime;
            altitudeDelta = -movementSpeed * Time.deltaTime;
            // pitch angle
            shipTargetPitch *= Quaternion.Euler(pitchAngle, 0, 0);
        }


        // left-right movement

        if (inputValues[1] && inputValues[3])
        {
            // do nothing basically
        }
        // A
        else if (inputValues[1])
        {
            shipTransform.Rotate(new Vector3(0, -turnSpeed * Time.deltaTime, 0), Space.Self);
            // add left roll
            shipTargetRoll *= Quaternion.Euler(0, 0, rollAngle);

        }
        // D
        else if (inputValues[3])
        {
            shipTransform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime, 0), Space.Self);
            // add right roll
            shipTargetRoll *= Quaternion.Euler(0, 0, -rollAngle);
        }

        float fauxGravityRotationAngle = computeFauxGravityRotationAngle(movementDelta, altitudeDelta, distanceFromGravityBoundObject);
        shipTransform.Rotate(fauxGravityRotationAngle, 0, 0);

        handleRotation(shipTargetPitch, shipTargetRoll);
    }

    private float computeFauxGravityRotationAngle(float movementDelta, float altitudeDelta, float initialDistanceFromGravityBoundObject)
    {
        // check wether the player moved forward or backwards to know later if the resulting angle should be positive or negative
        bool movedForward;
        if (movementDelta > 0) movedForward = true;
        else movedForward = false;

        // grabbing absolute value of the movement delta because negative values make no sense in this context
        movementDelta = Mathf.Abs(movementDelta);


        // some fucked calculations inc. to figure out how much to angle the ship as it goes around a sphere
        float distanceFromGravityBoundObjectAfterMovement = Vector3.Distance(gravityBoundGameObject.transform.position, shipTransform.position);
        // gets the normalized direction vector from center of the planet to the ship after it moved forward
        // and then sets the position of the ship again to preserve distance from planet
        Vector3 spaceShipDirection = (shipTransform.position - gravityBoundGameObject.transform.position).normalized;
        shipTransform.position = gravityBoundGameObject.transform.position + spaceShipDirection * (planetSize / 2 + heightAbovePlanet);
        // this represents the difference in altitude between the moment where the player is stationary and the moment after the movement is applied
        float realAltitudeDelta = distanceFromGravityBoundObjectAfterMovement - initialDistanceFromGravityBoundObject - altitudeDelta;
        // pythagorean theorem to find the distance traveled in the current frame
        float diagonal = Mathf.Sqrt(realAltitudeDelta * realAltitudeDelta + movementDelta * movementDelta);
        // the angle at which the ship should turn on the local X axis after the movement was applied
        // in an ABC right scalence triangle where ABC is the right angle, AB is the long side (movementDelta), BC (realAltitudeDelta) is the short side
        // this would be angle BAC
        float angle = Mathf.Asin(realAltitudeDelta / diagonal) * (180 / Mathf.PI);
        // fallback to 0 if the calculation fails (this happens when no movement is done)
        // also if the angle is 90 or -90 degrees it means the angle float underflowed because the realAltitudeDelta has a stupidly small value in it
        if (float.IsNaN(angle) || !(angle > -90 && angle < 90)) angle = 0;

        // if the player moved forward return the regular angle, negative angle for backwards movement
        if (movedForward) return angle;
        else return -angle;
    }

    private void handleRotation(Quaternion shipTargetPitch, Quaternion shipTargetRoll)
    {
        if (shipTargetPitch == Quaternion.identity && shipTargetRoll == Quaternion.identity)
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                    shipModelTransform.rotation,
                    shipTransform.rotation,
                    fallbackTurnSpeed * Time.deltaTime);
        }
        if (shipTargetPitch != Quaternion.identity)
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                shipModelTransform.rotation,
                shipTransform.rotation * shipTargetPitch,
                pitchTurnSpeed * Time.deltaTime);
        }
        if (shipTargetRoll != Quaternion.identity)
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                shipModelTransform.rotation,
                shipTransform.rotation * shipTargetRoll,
                rollTurnSpeed * Time.deltaTime);
        }
        if (shipTargetPitch != Quaternion.identity && shipTargetRoll != Quaternion.identity)
        {
            shipModelTransform.rotation = Quaternion.Slerp(
                shipModelTransform.rotation,
                shipTransform.rotation * shipTargetRoll,
                pitchTurnSpeed * Time.deltaTime); // using pitch turn speed in case more than 1 axis is turned at the same time because pitch is the most gentle rotation
        }
    }
}
