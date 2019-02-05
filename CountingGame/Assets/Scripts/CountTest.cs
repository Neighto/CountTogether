using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTest : MonoBehaviour
{

    public int count = 0;
    public bool canCount = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canCount)
        {
            count++;
        }
    }
}
