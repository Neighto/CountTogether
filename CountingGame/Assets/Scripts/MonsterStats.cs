using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    //Adjustable variables
    public float speed;
    public int quantity;

    private Transform xform;
    private bool changeLane = false;

    void Start()
    {
        xform = this.GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.position.x < this.gameObject.transform.position.x && !changeLane)
        {
            changeLane = true;
        }
    }

    void Update()
    {
        if (changeLane)
        {
            xform.position = new Vector3(xform.position.x, xform.position.y, xform.position.z - (speed * Time.deltaTime));
        }
        else
        {
            xform.position = new Vector3(xform.position.x - (speed * Time.deltaTime), xform.position.y, xform.position.z);
        }
    }
}
