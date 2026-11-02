using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public GameObject player;

    void FixedUpdate()
    {
        CameraPos();
    }

    void CameraPos()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var difference = mousePos - player.transform.position;
        var midpoint = difference/2 + player.transform.position;
        transform.position = midpoint;

    }
}
