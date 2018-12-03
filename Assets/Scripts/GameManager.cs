using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject travellingAgent;
    public GameObject wanderAgent;
    public GameObject socialAgent;

    public int numObjects;

    public int numTravellingAgents;
    public int numWanderAgents;
    public int numSocialAgents;

    public GameObject entranceDoorway;
    public GameObject[] exitDoorways;

    public static GameManager INSTANCE = null;
    private GameManager() { }

    private void Awake()
    {
        if (INSTANCE == null) INSTANCE = this;
        else if (INSTANCE != this) Destroy(this);   // ensures Singleton
    }

    // Use this for initialization
    void Start () {
        SpawnObjects();
        SpawnAgent(travellingAgent, numTravellingAgents);
        SpawnAgent(wanderAgent, numWanderAgents);
        SpawnAgent(socialAgent, numSocialAgents);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnObjects()
    {
        for(int i = 0; i < numObjects; i++)
        {
            // spawn the object??
        }
    }

    void SpawnAgent(GameObject agent, int num)
    {
        Vector3 t = entranceDoorway.transform.position;
        Vector3 pos = new Vector3(t.x, 0, t.z);

        if (agent.CompareTag("Traveller"))  // spawn traveller agents at doorway
        {
            for (int i = 0; i < num; i++)
            {
                var a = Instantiate(agent, pos, Quaternion.identity);
            }
        }
        else    // spawn the other agents randomly?
        {
            for (int i = 0; i < num; i++)
            {
                var a = Instantiate(agent);
            }
        }
    }

}
