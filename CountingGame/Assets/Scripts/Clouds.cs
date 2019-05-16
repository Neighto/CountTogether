using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    //Clouds transform.x move left until offscreen and resets to the right
    private Transform xform;
    private float speed = 2f;

    private void Start()
    {
        xform = this.GetComponent<Transform>();
    }

    void Update()
    {
        xform.position = new Vector3(xform.position.x - (speed * Time.deltaTime), xform.position.y, xform.position.z);
        if (xform.position.x < -77)
        {
            xform.position = new Vector3(67, xform.position.y, xform.position.z);
        }
    }
}
