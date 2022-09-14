using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDirection : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.blue);
    }
}
