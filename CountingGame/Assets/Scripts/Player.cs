using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int score;
    private int playerCount = 0;

    public int GetPlayerCount()
    {
        return playerCount;
    }

    public void ResetPlayerCount()
    {
        playerCount = 0;
    }

    public void AddCount()
    {
        playerCount++;
    }

}
