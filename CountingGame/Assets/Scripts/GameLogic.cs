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
    private ReadyCubes readyCubes;

    private Animator[] readyAnims;

    private Text timerText;
    private Text timerTextShadow;

    private readonly int maxPlayers = 8;

    public int numberOfPlayers;
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
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) p.GetComponent<Player>().score = 0;
        for (int i = 0; i < numberOfPlayers; i++) readyAnims[i].SetBool("Joined", true);

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
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);
            
            if (numberOfPlayers <= maxPlayers)
            {
                if (numberOfPlayers == 1) AirConsole.instance.Message(device_id, "Menu"); //player one can choose to start!
                else SetWaitScreens(false); //tell players to sit tight and wait!

                for (int i = 0; i < numberOfPlayers; i++) readyAnims[i].SetBool("Joined", true);
            }
            else
            {
                AirConsole.instance.Message(device_id, "Error");
            }
        }
        else
        {
            AirConsole.instance.Message(device_id, "Error");
        }
    }

    void OnDisconnect(int device_id)
    {
        if (!inGame)
        {
            readyCubes.allReady = false;
            numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
            AirConsole.instance.SetActivePlayers(numberOfPlayers);

            if (numberOfPlayers == 1) AirConsole.instance.Message(AirConsole.instance.GetActivePlayerDeviceIds[0], "Menu"); //player one can choose to start!
            else SetWaitScreens(false); //tell players to sit tight and wait!

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

    public void SetWaitScreens(bool andMain)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
            if (i >= maxPlayers) AirConsole.instance.Message(device_id, "Error");
            else AirConsole.instance.Message(device_id, "Wait" + (i + 1));
        }
        if (!andMain) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0), "Menu");
    }

    public void SetMenuScreens()
    {
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0), "Menu");
    }

    IEnumerator AllReadyDelay()
    {
        for (int i = 5; i > 0; i--) //while everybody is ready OR timer ends
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
            SetWaitScreens(true);
            SceneManager.LoadScene("Game");
        }
        else Debug.Log("Not everyone is ready!");

    }

    //get nickname as user joins
    public void SetNames(Text[] playerNameTexts) //was text
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            string name = AirConsole.instance.GetNickname(AirConsole.instance.GetActivePlayerDeviceIds[i]); //may or may not work
            if (name != null)
            {
                playerNameTexts[i].text = name;
            }
        }
    }
 
#endif
}
