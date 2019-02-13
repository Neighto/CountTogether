using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateUI : MonoBehaviour
{

    //UI Elements to Move
    public RectTransform playersPanel; //PLAYERS PANEL
    public RectTransform playersPanelInit;
    public RectTransform playersPanelDest;

    public Image darkenBG;

    //Basic Vars
    public float speed;
    private bool movePlayPanIn = false;
    private bool movePlayPanOut = true;
    private bool dark = true;

    //SHIFT CALLS (CALL BRINGS IN / CALL BRINGS OUT)

    public void ShiftPlayersPanel()
    {
        movePlayPanIn = !movePlayPanIn;
        movePlayPanOut = !movePlayPanOut;
    }

    //Darken Background for UI
    public void ShiftDarkenBackground()
    {
        dark = !dark;
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

        Color tempcolor = darkenBG.color;

        if (dark == false)
        {
            tempcolor.a = Mathf.MoveTowards(tempcolor.a, 0, Time.deltaTime);
            darkenBG.color = tempcolor;
        }
        else
        {
            tempcolor.a = Mathf.MoveTowards(tempcolor.a, 0.4f, Time.deltaTime);
            darkenBG.color = tempcolor;
        }


    }
}
