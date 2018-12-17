using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelAgentController : SteerBehaviour {
    
    public Transform target;
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

    public void Respawn()
    {
        GameObject doorway = GameManager.INSTANCE.entranceDoorway;
        transform.position = new Vector3(doorway.transform.position.x, 0.225f, doorway.transform.position.z);
        timer = 0.0f;
    }
}
