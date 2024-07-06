using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameObject mainCamera;
    public static GameObject cursorLocationInWorldSpace;

    void Awake()
    {
        mainCamera = GameObject.Find("Virtual Camera");
        cursorLocationInWorldSpace = GameObject.Find("CursorLocationInWorldSpace");
    }
}
