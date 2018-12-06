using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    public GameObject cube;
    public GameObject playingField;

    public int minScale;
    public int maxScale;

    public static ObstacleGenerator INSTANCE = null;
    private ObstacleGenerator() { }

    private void Awake()
    {
        if (INSTANCE == null) INSTANCE = this;
        else if (INSTANCE != this) Destroy(this);   // ensures Singleton
    }

    // Generate the number of objects indicated in GameManager
    public void GenerateAllObjects () {
        GameObject a = Instantiate(new GameObject());

        for (int i = 0; i < GameManager.INSTANCE.numObjects; i++)
        {
            GameObject g = GenerateObject();
            bool success = SpawnObject(g);
            if (!success)       // if there was no success in spawning an object, recreate the object and re-attempt spawn
            {
                Destroy(g);
                i--;
            }
            g.transform.parent = a.transform;
        }
	}

    // Generate an object that is the concatenation of 1-4 rectangles
    GameObject GenerateObject()
    {
        int x = Random.Range(0, 4);
        GameObject clone;

        clone = Instantiate(cube) as GameObject;
        clone.transform.localScale = new Vector3(Random.Range(minScale, maxScale), Random.Range(minScale, maxScale), Random.Range(minScale, maxScale));
        Bounds b = clone.GetComponent<Renderer>().bounds;

        for (int i = 0; i < x; i++)
        {
            GameObject clone2 = Instantiate(cube) as GameObject;
            clone2.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
            Bounds b2 = clone2.GetComponent<Renderer>().bounds;
            Vector3 random = new Vector3(Random.Range(b.min.x, b.max.x), 0, Random.Range(b.min.z, b.max.z));
            clone2.transform.localPosition = random;
            clone2.transform.parent = clone.transform;
        }
        clone.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        return clone;
    }

    // Spawn the object, return false if there isn't any space to spawn it (after 20 attempts)
    bool SpawnObject(GameObject g)
    {
        Bounds field = playingField.GetComponent<Renderer>().bounds;
        Bounds gBounds = g.GetComponent<Renderer>().bounds;

        int counter = 0;

        Vector3 random = new Vector3(Random.Range(field.min.x, field.max.x), gBounds.size.y / 2, Random.Range(field.min.z, field.max.z));
        while (Physics.CheckSphere(random, gBounds.size.magnitude * 0.65f)) // 
        {
            if(counter == 20)   // try to spawn the object 20 times
            {
                return false;
            }
            random = new Vector3(Random.Range(field.min.x, field.max.x), gBounds.size.y / 2, Random.Range(field.min.z, field.max.z));
            counter++;
        }

        g.transform.position = random;
        return true;
    }
}
