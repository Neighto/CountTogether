using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPlayerNames : MonoBehaviour
{
    //Depending on the number of players, will adjust the UI
    //Shows Player Guesses and Player Scores

    [HideInInspector] public Text[] playerCountTexts;
    [HideInInspector] public Text[] playerNameTexts;
    [HideInInspector] public Text[] playerPointTexts;
    [HideInInspector] public Animator[] playerAnimators;
    [HideInInspector] public RawImage[] playerColorImages;
    public Texture[] colorImages = new Texture[8];

    private GameObject[] adjustedPlayerDisplays;
    private readonly GameObject[] playerDisplays = new GameObject[13];

    // 1   2   9    3   10   4   C   5   11   6   12   7   8
    //[0] [1] [A2] [2] [A3] [3] [8] [4] [A5] [5] [A6] [6] [7]

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (Transform child in this.transform)
        {
            playerDisplays[i] = child.gameObject;
            playerDisplays[i].SetActive(false);
            i++;
        }
    }

    public void Setup(int numberOfPlayers) //called from GameController
    {
        adjustedPlayerDisplays = new GameObject[numberOfPlayers];

        if (numberOfPlayers == 1)
        {
            playerDisplays[8].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[8];
        }
        else if (numberOfPlayers == 2)
        {
            playerDisplays[3].SetActive(true);
            playerDisplays[4].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[3];
            adjustedPlayerDisplays[1] = playerDisplays[4];
        }
        else if (numberOfPlayers == 3)
        {
            playerDisplays[2].SetActive(true);
            playerDisplays[8].SetActive(true);
            playerDisplays[5].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[2];
            adjustedPlayerDisplays[1] = playerDisplays[8];
            adjustedPlayerDisplays[2] = playerDisplays[5];
        }
        else if (numberOfPlayers == 4)
        {
            playerDisplays[2].SetActive(true);
            playerDisplays[3].SetActive(true);
            playerDisplays[4].SetActive(true);
            playerDisplays[5].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[2];
            adjustedPlayerDisplays[1] = playerDisplays[3];
            adjustedPlayerDisplays[2] = playerDisplays[4];
            adjustedPlayerDisplays[3] = playerDisplays[5];
        }
        else if (numberOfPlayers == 5)
        {
            playerDisplays[9].SetActive(true);
            playerDisplays[10].SetActive(true);
            playerDisplays[8].SetActive(true);
            playerDisplays[11].SetActive(true);
            playerDisplays[12].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[9];
            adjustedPlayerDisplays[1] = playerDisplays[10];
            adjustedPlayerDisplays[2] = playerDisplays[8];
            adjustedPlayerDisplays[3] = playerDisplays[11];
            adjustedPlayerDisplays[4] = playerDisplays[12];
        }
        else if (numberOfPlayers == 6)
        {
            playerDisplays[1].SetActive(true);
            playerDisplays[2].SetActive(true);
            playerDisplays[3].SetActive(true);
            playerDisplays[4].SetActive(true);
            playerDisplays[5].SetActive(true);
            playerDisplays[6].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[1];
            adjustedPlayerDisplays[1] = playerDisplays[2];
            adjustedPlayerDisplays[2] = playerDisplays[3];
            adjustedPlayerDisplays[3] = playerDisplays[4];
            adjustedPlayerDisplays[4] = playerDisplays[5];
            adjustedPlayerDisplays[5] = playerDisplays[6];
        }
        else if (numberOfPlayers == 7)
        {
            playerDisplays[0].SetActive(true);
            playerDisplays[9].SetActive(true);
            playerDisplays[10].SetActive(true);
            playerDisplays[8].SetActive(true);
            playerDisplays[11].SetActive(true);
            playerDisplays[12].SetActive(true);
            playerDisplays[7].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[0];
            adjustedPlayerDisplays[1] = playerDisplays[9];
            adjustedPlayerDisplays[2] = playerDisplays[10];
            adjustedPlayerDisplays[3] = playerDisplays[8];
            adjustedPlayerDisplays[4] = playerDisplays[11];
            adjustedPlayerDisplays[5] = playerDisplays[12];
            adjustedPlayerDisplays[6] = playerDisplays[7];
        }
        else if (numberOfPlayers == 8)
        {
            playerDisplays[0].SetActive(true);
            playerDisplays[1].SetActive(true);
            playerDisplays[2].SetActive(true);
            playerDisplays[3].SetActive(true);
            playerDisplays[4].SetActive(true);
            playerDisplays[5].SetActive(true);
            playerDisplays[6].SetActive(true);
            playerDisplays[7].SetActive(true);
            adjustedPlayerDisplays[0] = playerDisplays[0];
            adjustedPlayerDisplays[1] = playerDisplays[1];
            adjustedPlayerDisplays[2] = playerDisplays[2];
            adjustedPlayerDisplays[3] = playerDisplays[3];
            adjustedPlayerDisplays[4] = playerDisplays[4];
            adjustedPlayerDisplays[5] = playerDisplays[5];
            adjustedPlayerDisplays[6] = playerDisplays[6];
            adjustedPlayerDisplays[7] = playerDisplays[7];
        }

        SetPublicPlayerArrays();
    }

    void SetPublicPlayerArrays()
    {
        int l = adjustedPlayerDisplays.Length;
        playerCountTexts = new Text[l];
        playerNameTexts = new Text[l];
        playerAnimators = new Animator[l];
        playerPointTexts = new Text[l];
        playerColorImages = new RawImage[l];

        for (int i = 0; i < l; i++)
        {
            playerCountTexts[i] = adjustedPlayerDisplays[i].transform.GetChild(2).GetComponent<Text>();
            playerNameTexts[i] = adjustedPlayerDisplays[i].transform.GetChild(1).GetComponent<Text>();
            playerAnimators[i] = adjustedPlayerDisplays[i].transform.GetChild(3).GetComponent<Animator>();
            playerPointTexts[i] = adjustedPlayerDisplays[i].transform.GetChild(3).GetComponent<Text>();
            playerColorImages[i] = adjustedPlayerDisplays[i].transform.GetChild(0).GetComponent<RawImage>();
            playerColorImages[i].texture = colorImages[i];
        }
    }

}
