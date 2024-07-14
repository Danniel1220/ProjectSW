using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameObject mainCamera;
    public static GameObject cursorLocationInWorldSpace;
    public static SpaceshipMovementController spaceshipMovementController;

    void Awake()
    {
        mainCamera = GameObject.Find("Virtual Camera");
        cursorLocationInWorldSpace = GameObject.Find("CursorLocationInWorldSpace");
        spaceshipMovementController = GameObject.Find("Spaceship").transform.GetComponent<SpaceshipMovementController>();

        validate();
    }

    private void validate()
    {
       if (mainCamera == null) { Debug.LogError("Error in GameManagers: could not grab refference for " + "mainCamera"); }
       if (cursorLocationInWorldSpace == null) { Debug.LogError("Error in GameManagers: could not grab refference for " + "cursorLocationInWorldSpace"); }
       if (spaceshipMovementController == null) { Debug.LogError("Error in GameManagers: could not grab refference for " + "spaceshipMovementController"); }
    }
}
