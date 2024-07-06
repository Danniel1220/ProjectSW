using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursorWorldSpace : MonoBehaviour
{
    private new Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        RaycastHit raycast;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out raycast))
        {
            transform.position = raycast.point;
        }    
    }
}
