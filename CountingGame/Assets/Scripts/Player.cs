using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isReady = false;
    public int score;
    private int playerCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

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
