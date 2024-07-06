using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCameraPan : MonoBehaviour
{
    Transform cameraAnchorTransform;
    Vector2 mouseMovement;

    void Start()
    {
        
    }

    void Update()
    {
        mouseMovement.x = Input.GetAxis("Mouse X");
        mouseMovement.y = Input.GetAxis("Mouse Y");

    }
}
