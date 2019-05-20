using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    //Adjustable variables
    public float speed;
    public int quantity;
    public int deathTime;
    private bool goingRight = false;
    private Transform xform;

    void Start()
    {
        xform = this.GetComponent<Transform>();
        Destroy(this.gameObject, deathTime);
        if (Random.Range(0, 10) == 7)
        {
            speed = 2 * speed;
        }
        else
        {
            speed = Random.Range(0.9f * speed, 1.2f * speed);
        }
    }

    public void ChangeDirection()
    {
        transform.RotateAround(transform.position, transform.up, 180f);
        goingRight = !goingRight;
    }

    void Update()
    {
        if (goingRight)
        {
            xform.position = new Vector3(xform.position.x + (speed * Time.deltaTime), xform.position.y, xform.position.z);
        }
        else
        {
            xform.position = new Vector3(xform.position.x - (speed * Time.deltaTime), xform.position.y, xform.position.z);
        }
    }
}
