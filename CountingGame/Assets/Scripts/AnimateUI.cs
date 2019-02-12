using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateUI : MonoBehaviour
{

    //UI Elements to Move
    public RectTransform playersPanel; //PLAYERS PANEL
    public RectTransform playersPanelInit;
    public RectTransform playersPanelDest;

    //Basic Vars
    public float speed;
    private bool movePlayPanIn = false;
    private bool movePlayPanOut = true;

    //SHIFT CALLS (CALL BRINGS IN / CALL BRINGS OUT)

    public void ShiftPlayersPanel()
    {
        movePlayPanIn = !movePlayPanIn;
        movePlayPanOut = !movePlayPanOut;
    }

    void Update()
    {
        if (movePlayPanIn)
        {
            playersPanel.position = Vector2.MoveTowards(playersPanel.position, playersPanelDest.position, speed * Time.deltaTime);
        }
        else if (movePlayPanOut)
        {
            playersPanel.position = Vector2.MoveTowards(playersPanel.position, playersPanelInit.position, speed * Time.deltaTime);
        }
    }
}
