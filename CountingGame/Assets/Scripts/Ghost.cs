using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float period;
    private float time;
    private float rand;
    private Transform xform;

    void Start()
    {
        xform = this.GetComponent<Transform>();
        //xform.position = new Vector3(xform.position.x, xform.position.y + rand, xform.position.z);
        rand = Random.Range(3f, 9.0f);
    }

    private void Update()
    {
        time += Time.deltaTime;
        float phase = Mathf.Sin(time / period) + rand;
        xform.localPosition = new Vector2(xform.position.x, phase);
    }


}
