using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyCubes : MonoBehaviour
{

    public bool allReady = false;
    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        gameLogic.FindVariables();
    }

}
