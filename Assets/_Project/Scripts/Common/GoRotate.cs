using UnityEngine;

public class GoRotate : MonoBehaviour
{
    [Header("Attributes")] 
    public bool ignoreTimeScale;
    public float speed = 1f;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    public void FixedUpdate()
    {
        var transformTemp = transform;
        if (rotateX)
        {
            transform.RotateAround(transformTemp.position, Vector3.right, Time.deltaTime * 90f * speed);
        }

        if (rotateY)
        {
            transform.RotateAround(transformTemp.position, Vector3.up, Time.deltaTime * 90f * speed);
        }

        if (rotateZ)
        {
            transform.RotateAround(transformTemp.position, transformTemp.forward, Time.deltaTime * 90f * speed);
        }
    }
}
