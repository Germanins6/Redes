using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour
{

    public float speed = 2.0f;
    public int maxSpheres = 5;
    Rigidbody sphereRB;
    private List<GameObject> gameObjects;


  
    // Start is called before the first frame update
    void Start()
    {
        gameObjects = new List<GameObject>();
        Debug.Log(gameObjects.Count);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if we can create simultaneous spheres
        if (gameObjects.Count <= maxSpheres)
        {
            //Once left click pressed
            if (Input.GetMouseButtonDown(0))
            {
                Debug.LogWarning("Creating Sphere");
                //Create GO
                GameObject temporalSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gameObjects.Add(temporalSphere);

                temporalSphere.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                sphereRB = temporalSphere.AddComponent<Rigidbody>();
                sphereRB.useGravity = false;
                StartCoroutine(SphereSpawner(temporalSphere));
            }
        }
    }

    IEnumerator SphereSpawner(GameObject sphere)
    {
        Debug.Log("Started corutine at: " + Time.time);

        //Moving sphere during 5 secs before destroy
        sphere.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
 
        yield return new WaitForSeconds(5);
        Destroy(sphere);
        Debug.Log("5sec passed");
    }
}
