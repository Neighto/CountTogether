using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float period;
    private float time;
    private Transform xform;

    void Start()
    {
        xform = this.GetComponent<Transform>();
        int rand = Random.Range(0, 6);
        xform.position = new Vector2(xform.position.x, xform.position.y + 3f + rand);
    }

    private void Update()
    {
        time += Time.deltaTime;
        float phase = Mathf.Sin(time / period);
        xform.localPosition = new Vector2(xform.position.x, phase);
    }


}
