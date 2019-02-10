using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawner : MonoBehaviour
{
    public GameObject monster1;
    private float nextSpawn = 6.0f;
    private float spawnRate = 12.0f;

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = spawnRate + Time.time;
            GameObject mon = Instantiate(monster1, transform.position, transform.rotation);
            Destroy(mon, 8f);
        }
    }
}
