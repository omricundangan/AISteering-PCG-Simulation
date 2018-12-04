using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject travellingAgent;
    public GameObject wanderAgent;
    public GameObject socialAgent;

    public float minSpeed;
    public float maxSpeed;

    public int numObjects;
    public int numTravellingAgents;
    public int numWanderAgents;
    public int numSocialAgents;

    public GameObject playingField;
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
        SpawnAgent(wanderAgent, numWanderAgents);
        SpawnAgent(socialAgent, numSocialAgents);
        StartCoroutine(SpawnTraveller(travellingAgent, numTravellingAgents));
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

    void CreateObject()
    {

    }

    void SpawnAgent(GameObject agent, int num)
    {
        Bounds b = playingField.GetComponent<Renderer>().bounds;
        Vector3 t = entranceDoorway.transform.position;

        for (int i = 0; i < num; i++)
        {
            var a = Instantiate(agent, new Vector3(Random.Range(b.min.x, b.max.x), 0.225f, Random.Range(b.min.z, b.max.z)), Quaternion.identity);
            if(a.CompareTag("Wanderer"))
                a.GetComponent<WanderAgentController>().maxSpeed = Random.Range(minSpeed, maxSpeed);
            else if(a.CompareTag("Socializer"))
                a.GetComponent<SocialAgentController>().maxSpeed = Random.Range(minSpeed, maxSpeed);
        }
    }

    public IEnumerator SpawnTraveller(GameObject agent, int num)
    {
        Vector3 t = entranceDoorway.transform.position;
        Vector3 pos = new Vector3(t.x, 0.225f, t.z);

        for (int i = 0; i < num; i++)
        {
            var a = Instantiate(agent, pos, Quaternion.identity);
            a.GetComponent<TravelAgentController>().maxSpeed = Random.Range(minSpeed, maxSpeed);
            yield return new WaitForSeconds(2);
        }
    }

}
