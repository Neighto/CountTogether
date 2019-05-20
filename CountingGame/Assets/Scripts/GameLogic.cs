using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;

    private GameObject readySpots;
    private Animator[] readyAnims;
    private Text timerText;
    private Text timerTextShadow;

    [HideInInspector] public int numberOfPlayers;
    [HideInInspector] public bool adShowing = false;
    private readonly int maxPlayers = 8;
    private bool inGame = false;
    private bool allReady = false;

#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        if (instance == null) //Only allow one instance of GameLogic
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

    //Find Variables as a separate function so LogicReload Script can call it on return to Lobby Scene
    public void FindVariables()
    {
        inGame = false;
        readySpots = GameObject.Find("ReadySpots");
        readyAnims = readySpots.GetComponentsInChildren<Animator>();
        timerText = GameObject.Find("timerText").GetComponent<Text>();
        timerTextShadow = GameObject.Find("timerTextShadow").GetComponent<Text>();
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) p.GetComponent<Player>().score = 0;
        for (int i = 0; i < numberOfPlayers && i < maxPlayers; i++) readyAnims[i].SetBool("Joined", true);
    }

    void OnReady(string code)
    {
        //Initialize Game State
        JObject newGameState = new JObject
        {
            { "view", new JObject() }
        };
        AirConsole.instance.SetCustomDeviceState(newGameState);
    }

    void OnConnect(int device_id)
    {
        if (!inGame)
        {
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);

            if (numberOfPlayers <= maxPlayers)
            {
                if (numberOfPlayers == 1) AirConsole.instance.Message(device_id, "Menu"); //player one can choose to start!
                else SetWaitScreens(); //tell players to sit tight and wait!

                for (int i = 0; i < numberOfPlayers; i++) readyAnims[i].SetBool("Joined", true);
            }
            else //Too many players!
            {
                AirConsole.instance.Message(device_id, "Error");
            }
        }
        else //Game has started already!
        {
            AirConsole.instance.Message(device_id, "Error");
        }
    }

    void OnDisconnect(int device_id)
    {
        if (!inGame)
        {
            allReady = false;
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);

            if (numberOfPlayers == 1) AirConsole.instance.Message(AirConsole.instance.GetActivePlayerDeviceIds[0], "Menu"); //player one can choose to start!
            else SetWaitScreens(); //tell players to sit tight and wait!

            if (readyAnims[numberOfPlayers] != null) readyAnims[numberOfPlayers].SetBool("Joined", false);
        }
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
                 player.playerCount++; //Add 1 to count
            }

            if (action == "menu-button")
            {
                allReady = !allReady;
                if (allReady) StartCoroutine(AllReadyDelay());          
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

    //Set functions

    public void SetCreaturesOnScreens(string creature) //creature always start with '_'
    {
        for (int i = 0; i < numberOfPlayers && i < maxPlayers; i++)
        {
            int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
            AirConsole.instance.Message(device_id, creature);
        }
    }

    public void SetCountScreens()
    {
        for (int i = 0; i < numberOfPlayers && i < maxPlayers; i++)
        {
            int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
            AirConsole.instance.Message(device_id, "Count" + (i + 1));
        }
    }

    public void SetWaitScreens()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
            if (i == 0 && !inGame) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0), "Menu");
            else if (i < maxPlayers) AirConsole.instance.Message(device_id, "Wait" + (i + 1));
            else AirConsole.instance.Message(device_id, "Error");
        }
    }

    public void SetMenuScreens()
    {
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0), "Menu");
    }

    public void SetNames(Text[] playerNameTexts) //was text
    {
        for (int i = 0; i < playerNameTexts.Length && i < maxPlayers; i++)
        {
            string name = AirConsole.instance.GetNickname(AirConsole.instance.GetActivePlayerDeviceIds[i]);
            if (name != null)
            {
                playerNameTexts[i].text = name;
            }
        }
    }

    //End function

    IEnumerator AllReadyDelay()
    {
        for (int i = 5; i > 0; i--) //while everybody is ready OR timer ends
        {
            timerText.text = "START IN " + i;
            timerTextShadow.text = "START IN " + i;
            yield return new WaitForSeconds(1f);
            if (allReady == false)
            {
                timerText.text = "";
                timerTextShadow.text = "";
                break;
            }
        }
        if (allReady) //The game is starting NOW
        {
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);
            inGame = true;
            SetWaitScreens();
            SceneManager.LoadScene("Game");
        }
    }

    //Ad functions

    public void GetAd() //request ad
    {
        AirConsole.instance.ShowAd();
    }

    void OnAdShow() //if ad is truly called
    {
        adShowing = true;
    }

    void OnAdComplete() //called if ad is closed
    {
        adShowing = false;
    }


#endif
}
