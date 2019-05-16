using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicReload : MonoBehaviour
{
    //SCRIPT EXISTS TO CALL GAMELOGIC UPON SCENE RELOAD
    private GameLogic gameLogic;
    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        gameLogic.FindVariables();
    }
}
