using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDirection : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        var position = transform.position;
        Debug.DrawLine(position, position + transform.forward, Color.red);
        Debug.DrawLine(position, position + transform.up, Color.yellow);
        Debug.DrawLine(position, position + transform.right, Color.blue);
    }
}
