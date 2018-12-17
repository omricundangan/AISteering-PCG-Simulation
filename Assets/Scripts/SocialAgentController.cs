using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialAgentController : SteerBehaviour{

    public float CIRCLE_RADIUS;
    public float CIRCLE_DISTANCE;
    public float ANGLE_CHANGE;

    private float WanderAngle = 0.0f;

    public Transform target;
    public bool socializing;

    // Use this for initialization
    void Start()
    {
        // Choose a random exit
        size = GetComponent<Renderer>().bounds.size.magnitude;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turn) TurnAround();
        else Steer();
    }

    void Steer()
    {
        if (maxSpeed == 0 || maxForce == 0) return;
        var steering = Vector3.zero;
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

    public void Respawn()
    {
        GameObject doorway = GameManager.INSTANCE.entranceDoorway;
        transform.position = new Vector3(doorway.transform.position.x, 0.225f, doorway.transform.position.z);
    }
}
