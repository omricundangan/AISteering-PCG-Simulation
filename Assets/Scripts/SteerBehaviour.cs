using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerBehaviour : MonoBehaviour {

    public float mass;
    public float maxForce;
    public float maxSpeed;
    public float MAX_SEE_AHEAD;
    public float MAX_AVOID_FORCE;

    protected Vector3 velocity;
    protected Rigidbody rb;
    protected float size;
    protected bool turn = false;

    protected Vector3 Seek(Transform target)
    {
        Vector3 t = target.transform.position;
        t = new Vector3(t.x, 0.225f, t.z);

        var desiredVelocity = t - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }

    protected Vector3 Seek(Vector3 target)
    {
        Vector3 t = target;
        t = new Vector3(t.x, 0.225f, t.z);

        var desiredVelocity = t - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }

    // SphereCast in front of the agent a distance of MAX_SEE_AHEAD
    // If there is sometihng, calculate the avoidance force based on the object
    // otherwise, check if the agent is currently colliding with something and do the same
    // Otherwise, no avoid force
    protected Vector3 CollisionAvoidance()
    {
        var ahead = transform.position + (velocity.normalized * MAX_SEE_AHEAD);

        RaycastHit hit;
        Vector3 avoidance_force = Vector3.zero;

        if (Physics.SphereCast(transform.position, size * 0.5f, velocity, out hit, MAX_SEE_AHEAD))
        {
            GameObject go = hit.transform.gameObject;
            if (go == GameManager.INSTANCE.exitDoorways[0] || go == GameManager.INSTANCE.exitDoorways[1]) return Vector3.zero;
            var obstacle_center = go.transform.GetComponent<Renderer>().bounds.center;
            obstacle_center = new Vector3(obstacle_center.x, 0.225f, obstacle_center.z);

            avoidance_force = ahead - obstacle_center;
            avoidance_force = avoidance_force.normalized * MAX_AVOID_FORCE;
        }
        else if (Physics.Raycast(transform.position, velocity, out hit, size))
        {
            if (!hit.transform.gameObject.CompareTag("Obstacle")) return avoidance_force;
            turn = true;
        }
        return avoidance_force;
    }

    protected void TurnAround()
    {
        transform.Rotate(new Vector3(0, 180, 0));
        velocity = velocity * -1;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        rb.MovePosition(transform.position + (velocity * Time.deltaTime));
        transform.forward = velocity.normalized;
        turn = false;
    }


}
