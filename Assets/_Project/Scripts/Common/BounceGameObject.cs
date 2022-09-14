using UnityEngine;

public class BounceGameObject : MonoBehaviour
{
    [Header("Attributes")]
    public bool IsRotate = false;
    public float DegreesPerSecond = 15.0f;
    public float Amplitude = 0.5f;
    public float Frequency = 1f;
    
    Vector3 posOffset;
    Vector3 tempPos;
    
    void Start () {
        posOffset = transform.position;
    }
 
    void FixedUpdate () {
        if (IsRotate)
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * DegreesPerSecond, 0f), Space.World);
        }
        
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * Frequency) * Amplitude;
 
        transform.position = tempPos;
    }
}
