using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelAgentController : MonoBehaviour {

    public float speed;
    public Transform exit;

	// Use this for initialization
	void Start () {
        // Choose a random exit
        exit = GameManager.INSTANCE.exitDoorways[Random.Range(0, GameManager.INSTANCE.exitDoorways.Length)].transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Respawn()
    {
        GameObject doorway = GameManager.INSTANCE.entranceDoorway;
        transform.position = doorway.transform.position;
    }
}
