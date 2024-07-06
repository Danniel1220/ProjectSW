using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipShellShooter : MonoBehaviour
{
    GameObject leftWingBarrel1ShootPoint;
    GameObject leftWingBarrel2ShootPoint;
    GameObject leftWingBarrel3ShootPoint;
    GameObject leftWingBarrel4ShootPoint;
    GameObject rightWingBarrel1ShootPoint;
    GameObject rightWingBarrel2ShootPoint;
    GameObject rightWingBarrel3ShootPoint;
    GameObject rightWingBarrel4ShootPoint;

    GameObject shellPrefab;

    GameObject cursorLocationInWorldSpace;


    void Start()
    {
        leftWingBarrel1ShootPoint = GameObject.Find("Spaceship/Model/LWing/ShellShootPoint1");
        leftWingBarrel2ShootPoint = GameObject.Find("Spaceship/Model/LWing/ShellShootPoint2");
        leftWingBarrel3ShootPoint = GameObject.Find("Spaceship/Model/LWing/ShellShootPoint3");
        leftWingBarrel4ShootPoint = GameObject.Find("Spaceship/Model/LWing/ShellShootPoint4");

        rightWingBarrel1ShootPoint = GameObject.Find("Spaceship/Model/RWing/ShellShootPoint1");
        rightWingBarrel2ShootPoint = GameObject.Find("Spaceship/Model/RWing/ShellShootPoint2");
        rightWingBarrel3ShootPoint = GameObject.Find("Spaceship/Model/RWing/ShellShootPoint3");
        rightWingBarrel4ShootPoint = GameObject.Find("Spaceship/Model/RWing/ShellShootPoint4");

        shellPrefab = Resources.Load("Prefabs/Shell") as GameObject;

        cursorLocationInWorldSpace = GameManagers.cursorLocationInWorldSpace;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("lmbutton");

            shootShell(leftWingBarrel1ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(leftWingBarrel2ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(leftWingBarrel3ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(leftWingBarrel4ShootPoint, cursorLocationInWorldSpace.transform.position);

            shootShell(rightWingBarrel1ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(rightWingBarrel2ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(rightWingBarrel3ShootPoint, cursorLocationInWorldSpace.transform.position);
            shootShell(rightWingBarrel4ShootPoint, cursorLocationInWorldSpace.transform.position);
        }
    }

    private void shootShell(GameObject barrel, Vector3 target)
    {
        GameObject shellGameObject = Instantiate(
                shellPrefab,
                barrel.transform.position,
                Quaternion.LookRotation(target - barrel.transform.position));
        ShellBehaviour shellScript = shellGameObject.AddComponent<ShellBehaviour>();
        shellScript.setTargetPosition(target);
    }
}
