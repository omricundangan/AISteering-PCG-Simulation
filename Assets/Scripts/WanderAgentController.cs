using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAgentController : MonoBehaviour
{
    public float mass;
    public float maxForce;
    public float maxSpeed;
    public float MAX_SEE_AHEAD;
    public float MAX_AVOID_FORCE;
    public float CIRCLE_RADIUS;
    public float CIRCLE_DISTANCE;
    public float ANGLE_CHANGE;
    public float VisionRadius;
    public float MaxChaseDistance;

    private GameObject traveller;
    private bool defending = false;
    private bool turn = false;

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
    void Update()
    {
        if (!defending) CheckForTraveller();
        if (turn) TurnAround();
        else Steer();
    }

    // Sum steering forces up then add to velocity.
    void Steer()
    {
        var steering = Vector3.zero;
        if (defending) steering += BlockTraveller();
        else steering += Wander();
        steering += CollisionAvoidance();
        steering = Vector3.ClampMagnitude(steering, maxForce); // truncate(steering, max_force)
        steering = steering / mass; // acceleration
        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);

        rb.MovePosition(transform.position + (velocity * Time.deltaTime));
        transform.forward = velocity.normalized;

        Debug.DrawRay(transform.position, velocity.normalized * MAX_SEE_AHEAD, Color.black);
        // Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }

    // Find the closest traveller within the VisionRadius if there is one
    void CheckForTraveller() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, VisionRadius);
        GameObject closest = null;
        float distance = Mathf.Infinity;

        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.CompareTag("Traveller"))
            {
                Vector3 diff = c.gameObject.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    defending = true;
                    closest = c.gameObject;
                }
            }
        }
        traveller = closest;
    }

    Vector3 BlockTraveller()    // Block the traveller that we Checked for
    {
        var distance = (traveller.transform.position - transform.position).magnitude;
        if (distance > MaxChaseDistance)    // stop chasing traveller if too far
        {
            defending = false;
            traveller = null;
            return Wander();
        }

        Vector3 travellerGoal = traveller.GetComponent<TravelAgentController>().target.transform.position;
        travellerGoal = new Vector3(travellerGoal.x, 0.225f, travellerGoal.z);

        Vector3 midPointOfTravellerAndGoal = (travellerGoal + traveller.transform.position) / 2;

        return Seek(midPointOfTravellerAndGoal);
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

    // Calculate random displacement according to CIRCLE_RADIUS and ANGLE_CHANGE
    Vector3 Wander()
    {
        Vector3 circleCenter = velocity.normalized * CIRCLE_DISTANCE;
        Vector3 displacement = new Vector3(0,0,-1) * CIRCLE_RADIUS;

        WanderAngle += Random.Range(-ANGLE_CHANGE*0.5f, ANGLE_CHANGE*0.5f);    // get random angle

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
        var ahead = transform.position + (velocity.normalized * MAX_SEE_AHEAD);

        RaycastHit hit;
        Vector3 avoidance_force = Vector3.zero;

        if (Physics.SphereCast(transform.position, size / 2, transform.forward, out hit, MAX_SEE_AHEAD))
        {
            GameObject go = hit.transform.gameObject;
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

    void TurnAround()
    {
        transform.Rotate(new Vector3(0, 180, 0));
        velocity = velocity * -1;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        rb.MovePosition(transform.position + (velocity * Time.deltaTime));
        transform.forward = velocity.normalized;
        turn = false;
    }
}
