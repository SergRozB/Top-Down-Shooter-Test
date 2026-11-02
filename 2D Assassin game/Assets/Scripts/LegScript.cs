using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegScript : MonoBehaviour
{
    Player_Movement movementScript;

    void Start()
    {
        movementScript = GetComponentInParent<Player_Movement>();
    }

    void Update()
    {
        Vector2 movementDirection = movementScript.moveDirection;
        if (movementDirection != Vector2.zero)
        {
            Debug.Log("mov:" + movementDirection);
            Quaternion targetRot = Quaternion.LookRotation(transform.forward, movementDirection);
            Vector3 vectorRot1 = targetRot.eulerAngles;
            targetRot = Quaternion.RotateTowards(transform.rotation, targetRot, 1080 * Time.deltaTime);
            transform.rotation = targetRot;
        }  
    }
}
