using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialAgentController : MonoBehaviour
{

    public float mass;
    public float maxForce;
    public float maxSpeed;
    public float MAX_SEE_AHEAD;
    public float MAX_AVOID_FORCE;
    public float CIRCLE_RADIUS;
    public float CIRCLE_DISTANCE;
    public float ANGLE_CHANGE;

    private float WanderAngle = 0.0f;

    private Vector3 velocity;
    public Transform target;
    private float size;
    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        // Choose a random exit
        size = GetComponent<Renderer>().bounds.size.magnitude;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Steer();
    }

    void Steer()
    {
        if (maxSpeed == 0 || maxForce == 0) return;
        var steering = Vector3.zero;
        if (target != null) steering += Seek(target);
        steering += Wander();
        steering += CollisionAvoidance();
        steering = Vector3.ClampMagnitude(steering, maxForce); // truncate(steering, max_force)
        steering = steering / mass; // acceleration
        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);

        //transform.position += velocity * Time.deltaTime;
        // rb.position = rb.position + (velocity * Time.deltaTime);
        rb.MovePosition(transform.position + (velocity * Time.deltaTime));
        transform.forward = velocity.normalized;

        Debug.DrawRay(transform.position, velocity.normalized * MAX_SEE_AHEAD, Color.black);
        // Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }

    Vector3 Seek(Transform target)
    {
        Vector3 t = target.transform.position;
        t = new Vector3(t.x, 0.225f, t.z);

        var desiredVelocity = t - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 t = target;
        t = new Vector3(t.x, 0.225f, t.z);

        var desiredVelocity = t - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }


    Vector3 Wander()
    {
        Vector3 circleCenter = velocity.normalized * CIRCLE_DISTANCE;
        Vector3 displacement = new Vector3(0, 0, -1) * CIRCLE_RADIUS;

        WanderAngle += Random.Range(-ANGLE_CHANGE * 0.5f, ANGLE_CHANGE * 0.5f);    // get random angle

        displacement = SetAngle(displacement, WanderAngle);

        return circleCenter + displacement;
    }

    Vector3 SetAngle(Vector3 v, float angle)
    {
        float len = v.magnitude;
        return new Vector3(Mathf.Cos(angle) * len, 0, Mathf.Sin(angle) * len);
    }


    Vector3 CollisionAvoidance()
    {
        // var dynamic_length = velocity.magnitude / maxSpeed;
        // var ahead = transform.position + (velocity.normalized * dynamic_length);

        var ahead = transform.position + (velocity.normalized * MAX_SEE_AHEAD);
        var ahead2 = transform.position + (velocity.normalized * (MAX_SEE_AHEAD * 0.5f));

        // Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Vector3 avoidance_force = Vector3.zero;

        if (Physics.SphereCast(transform.position, size / 2, transform.forward, out hit, MAX_SEE_AHEAD))
        {
            // if (Physics.Raycast(transform.position, fwd, out hit, MAX_SEE_AHEAD)) {
            GameObject go = hit.transform.gameObject;
            //print("detecting " + go);
            //print("avoiding " + go);
            var obstacle_center = go.transform.GetComponent<Renderer>().bounds.center;
            obstacle_center = new Vector3(obstacle_center.x, 0.225f, obstacle_center.z);

            avoidance_force = ahead - obstacle_center;
            avoidance_force = avoidance_force.normalized * MAX_AVOID_FORCE;
        }
        //else print(" ");

        return avoidance_force;
    }



    public void Respawn()
    {
        GameObject doorway = GameManager.INSTANCE.entranceDoorway;
        transform.position = new Vector3(doorway.transform.position.x, 0.225f, doorway.transform.position.z);
    }
}
