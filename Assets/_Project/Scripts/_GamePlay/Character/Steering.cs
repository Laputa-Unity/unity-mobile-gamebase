using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public static Vector3 Flee(Vector3 position, Vector3 target, float maxForce, Vector3 velocity, float mass)
    {
        var desiredVelocity = (target - position).normalized * maxForce;
        return (-desiredVelocity - velocity) / mass;
    }

    public static Vector3 Seek(Vector3 position, Vector3 target, float maxForce, Vector3 velocity, float mass)
    {
        var desiredVelocity = (target - position).normalized * maxForce;
        //var force = Vector3.ClampMagnitude(desiredVelocity - _velocity, maxForce);
        return (desiredVelocity - velocity) / mass;
    }

    public static Vector3 Arrival(Vector3 position, Vector3 target, float radius, float maxForce, Vector3 velocity, float mass)
    {
        var desiredVelocity = target - position;
        float distance = desiredVelocity.magnitude;
        if (distance < radius)
        {
            desiredVelocity = distance / radius * maxForce * desiredVelocity.normalized;
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * maxForce;
        }

        return (desiredVelocity - velocity) / mass;
    }

    public static Vector3 Wander(Vector3 position, Vector3 target, float maxRadius, float circleRadius, Vector3 velocity, float turnChance, ref Vector3 wanderForce)
    {
        if (position.magnitude > maxRadius)
        {
            var directionToCenter = (target - position).normalized;
            wanderForce = velocity.normalized + directionToCenter;
        }
        else if (Random.value < turnChance)
        {
            wanderForce = WanderFore(velocity, circleRadius);
        }
        
        return wanderForce;
    }

    private static Vector3 WanderFore(Vector3 velocity, float circleRadius)
    {
        var circleCenter = velocity.normalized;
        var randomPoint = Random.insideUnitCircle;

        var displacement = new Vector3(randomPoint.x, randomPoint.y) * circleRadius;
        displacement = Quaternion.LookRotation(velocity) * displacement;

        return circleCenter + displacement;
    }

    public static Vector3 Pursuit(Vector3 position, Vector3 target, float maxForce, Vector3 targetVelocity, float t, float mass)
    {
        Vector3 futurePosition = target + targetVelocity * t;
        return Seek(position, futurePosition, maxForce, targetVelocity, mass);
    }
    
    public static Vector3 Evade(Vector3 position, Vector3 target, float maxForce, Vector3 targetVelocity, float mass)
    {
        var t = (target - position).magnitude / maxForce;
        Vector3 futurePosition = target + targetVelocity * t;
        return Flee(position, futurePosition, maxForce, targetVelocity, mass);
    }

}
