using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateUI : MonoBehaviour
{

    //UI Elements to Move
    public RectTransform reminderPanel; //REMINDER PANEL
    public RectTransform reminderPanelInit;
    public RectTransform reminderPanelDest;
    public RectTransform playersPanel; //PLAYERS PANEL
    public RectTransform playersPanelInit;
    public RectTransform playersPanelDest;

    //Basic Vars
    public float speed;
    private bool moveRemPanIn = false;
    private bool moveRemPanOut = true;
    private bool movePlayPanIn = false;
    private bool movePlayPanOut = true;

    //SHIFT CALLS (CALL BRINGS IN / CALL BRINGS OUT)

    public void ShiftReminderPanel()
    {
        moveRemPanIn = !moveRemPanIn;
        moveRemPanOut = !moveRemPanOut;
    }

    public void ShiftPlayersPanel()
    {
        movePlayPanIn = !movePlayPanIn;
        movePlayPanOut = !movePlayPanOut;
    }

    void Update()
    {
        if (moveRemPanIn)
        {
            reminderPanel.position = Vector2.MoveTowards(reminderPanel.position, reminderPanelDest.position, speed * Time.deltaTime);
        }
        else if (moveRemPanOut)
        {
            reminderPanel.position = Vector2.MoveTowards(reminderPanel.position, reminderPanelInit.position, speed * Time.deltaTime);
        }

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
