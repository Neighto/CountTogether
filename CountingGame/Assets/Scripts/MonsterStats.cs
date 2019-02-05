using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : MonoBehaviour
{

    //Adjustable variables
    public float speed;
    public int quantity;

    private Transform xform;

    void Start()
    {
        xform = this.GetComponent<Transform>();
    }

    void Update()
    {
        xform.position = new Vector3(xform.position.x - (speed * Time.deltaTime), xform.position.y, xform.position.z);
    }
}
