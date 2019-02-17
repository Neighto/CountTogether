﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;

    public GameObject player;

    private GameObject readySpots;
    private ReadyCubes readyCubes;

    private Animator[] readyAnims;

    private Text timerText;
    private Text timerTextShadow;

    private int numberOfPlayers;
    private bool inGame = false;

#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    private void Start()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onDisconnect += OnDisconnect;
        FindVariables();
    }

    //Find Variables as a separate function so Ready Cubes Script can call it on return to Lobby Scene
    public void FindVariables()
    {
        inGame = false;
        readySpots = GameObject.Find("ReadySpots");
        readyAnims = readySpots.GetComponentsInChildren<Animator>();
        readyCubes = readySpots.GetComponent<ReadyCubes>();
        timerText = GameObject.Find("timerText").GetComponent<Text>();
        timerTextShadow = GameObject.Find("timerTextShadow").GetComponent<Text>();
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            p.GetComponent<Player>().score = 0;
            p.GetComponent<Player>().isReady = false;
            readyAnims[int.Parse(p.name)].SetBool("Joined", true);
        }

    }

    void OnReady(string code)
    {
        //Initialize Game State
        JObject newGameState = new JObject();
        newGameState.Add("view", new JObject());

        AirConsole.instance.SetCustomDeviceState(newGameState);
    }

    void OnConnect(int device_id)
    {
        if (!inGame)
        {
            int numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);

            if (numberOfPlayers <= 8)
            {
                if (numberOfPlayers == 1) AirConsole.instance.Message(device_id, "Menu"); //player one can choose to start!
                else WaitScreen(device_id); //tell players to sit tight and wait!

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    if (GameObject.Find(i.ToString()) == null)
                    {
                        GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
                        newPlayer.name = i.ToString();
                    }
                }
                int playerNum = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
                readyAnims[playerNum].SetBool("Joined", true);
            }
        }

    }

    void OnDisconnect(int device_id)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        readyAnims[active_player].SetBool("Joined", false);
        readyCubes.allReady = false;
        Destroy(GameObject.Find(active_player.ToString())); 

    }

    void OnMessage(int deviceId, JToken message)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId);
        string action = (string)message["action"];
        if (action != null)
        {
            if (action == "ingame-button")
            {
                 Player player = GameObject.Find("" + active_player).GetComponent<Player>();
                 player.AddCount();
            }

            if (action == "menu-button")
            {
                readyCubes.allReady = !readyCubes.allReady;
                if (readyCubes.allReady) StartCoroutine(AllReadyDelay());
            }
        }
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onConnect -= OnConnect;
            AirConsole.instance.onReady -= OnReady;
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    public void SetCountScreens()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i == 0) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count1");
            else if (i == 1) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count2");
            else if (i == 2) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count3");
            else if (i == 3) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count4");
            else if (i == 4) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count5");
            else if (i == 5) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count6");
            else if (i == 6) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count7");
            else if (i == 7) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Count8");
        }
    }

    public void SetWaitScreens()
    {
        for (int i = 0; i < numberOfPlayers;)
        {
            int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
            AirConsole.instance.Message(device_id, "Wait" + ++i);
        }
    }

    private void WaitScreen(int device_id)
    {
        int playerNum = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        AirConsole.instance.Message(device_id, "Wait" + ++playerNum);
    }

    public void SetMenuScreens()
    {
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0), "Menu");
    }

    IEnumerator AllReadyDelay()
    {
        for (int i = 5; i >= 0; i--) //while everybody is ready OR timer ends
        {
            timerText.text = "START IN " + i;
            timerTextShadow.text = "START IN " + i;
            yield return new WaitForSeconds(1f);
            if (readyCubes.allReady == false)
            {
                timerText.text = "";
                timerTextShadow.text = "";
                break;
            }
        }
        if (readyCubes.allReady) //The game is starting NOW
        {
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);
            inGame = true;
            SetWaitScreens();
            SceneManager.LoadScene("Game");
        }
        else Debug.Log("Not everyone is ready!");

    }
 
#endif
}
