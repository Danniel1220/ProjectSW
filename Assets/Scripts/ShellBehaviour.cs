using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : MonoBehaviour
{
    Vector3 targetPosition;
    float shellSpeed = 120f;

    void Start()
    {
        
    }

    void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, shellSpeed * Time.deltaTime);
    }

    public void setTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
