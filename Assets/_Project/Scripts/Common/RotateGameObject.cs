using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    [Header("Attributes")]
    [Range(0.1f,100f)] public float Speed = 1f;
    public bool IsRotationX;
    public bool IsRotationY;
    public bool IsRotationZ;
    public void FixedUpdate()
    {
        if (IsRotationX)
        {
            transform.RotateAround(transform.position, transform.right, Time.deltaTime * 90f * Speed);
        }

        if (IsRotationY)
        {
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f * Speed);
        }

        if (IsRotationZ)
        {
            transform.RotateAround(transform.position, transform.forward, Time.deltaTime * 90f * Speed);
        }
        
    }
}
