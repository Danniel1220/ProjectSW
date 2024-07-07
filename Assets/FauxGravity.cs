using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravity : MonoBehaviour
{
    Transform shipBodyTransform;
    GameObject gravityBoundGameObject;

    void Start()
    {
        shipBodyTransform = this.transform.Find("Model").transform;
        gravityBoundGameObject = GameObject.Find("Planet");
    }

    void Update()
    {
        shipBodyTransform.LookAt(gravityBoundGameObject.transform);
    }
}
