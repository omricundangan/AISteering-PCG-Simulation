using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelAgentController : MonoBehaviour {

    public float mass;
    public float maxForce;
    public float maxSpeed;
    public float MAX_SEE_AHEAD;
    public float MAX_AVOID_FORCE;
    private Vector3 velocity;
    public Transform target;
    private float size;
    private Rigidbody rb;
    private bool turn = false;
    public float timeOut;
    public float timer = 0.0f;  // public just for debugging purposes

	// Use this for initialization
	void Start () {
        // Choose a random exit
        target = GameManager.INSTANCE.exitDoorways[Random.Range(0, GameManager.INSTANCE.exitDoorways.Length)].transform;
        size = GetComponent<Renderer>().bounds.size.magnitude;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > timeOut) // timeOut adjustable in Traveller prefab
        {
            ToggleTarget();
            timer = 0.0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
        if (turn) TurnAround();
        else Steer();
    }

    void ToggleTarget()
    {
        if(target.gameObject == GameManager.INSTANCE.exitDoorways[0])
        {
            target = GameManager.INSTANCE.exitDoorways[1].transform;
        }
        else
        {
            target = GameManager.INSTANCE.exitDoorways[0].transform;
        }
    }

    // Sum Seek and Avoid forces into steering forces. Add to velocity and truncate based on max values.
    void Steer() {
        var steering = Vector3.zero;
        steering += Seek(target);
        //steering = CollisionAvoidance();
        steering = Vector3.ClampMagnitude(steering,  maxForce); // truncate(steering, max_force)
        steering = steering / mass; // acceleration
        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);
        transform.forward = velocity.normalized;

        // Apply seek force to the steering force and add to velocity.
        // Calculate avoidance force based on new velocity with the added seek
        steering = CollisionAvoidance();
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

    // SphereCast in front of the agent a distance of MAX_SEE_AHEAD
    // If there is sometihng, calculate the avoidance force based on the object
    // otherwise, check if the agent is currently colliding with something and do the same
    // Otherwise, no avoid force
    Vector3 CollisionAvoidance()
    {
        var ahead = transform.position + (velocity.normalized * MAX_SEE_AHEAD);

        RaycastHit hit;
        Vector3 avoidance_force = Vector3.zero;

        if (Physics.SphereCast(transform.position, size*0.5f, velocity, out hit, MAX_SEE_AHEAD))
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

    public void Respawn()
    {
        GameObject doorway = GameManager.INSTANCE.entranceDoorway;
        transform.position = new Vector3(doorway.transform.position.x, 0.225f, doorway.transform.position.z);
        timer = 0.0f;
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
