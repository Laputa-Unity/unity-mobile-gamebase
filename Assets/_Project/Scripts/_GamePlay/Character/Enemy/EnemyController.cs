using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float mass = 15;
    public float maxVelocity = 3;
    public float maxForce = 15;
    public float radius;

    private Vector3 _velocity = Vector3.zero;
    public Transform target;

    private void Update()
    {
        var force = Steering.Arrival(transform.position, target.position, radius, maxForce, _velocity, mass);
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxVelocity);
        transform.position += _velocity * Time.deltaTime;
    }

}
