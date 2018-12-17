using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAgentController : SteerBehaviour
{
    public float CIRCLE_RADIUS;
    public float CIRCLE_DISTANCE;
    public float ANGLE_CHANGE;
    public float VisionRadius;
    public float MaxChaseDistance;

    private GameObject traveller;
    private bool defending = false;

    private float WanderAngle = 0.0f;
    public Transform target;

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

}
