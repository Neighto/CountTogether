using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;
    public GameObject player;

    private GameObject readySpots;
    private ReadyCubes readyCubes;
    private Animator[] readyAnims;
    private Text timerText;
    private Text timerTextShadow;
    private bool allTrue = false;
    private bool inGame = false;

#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        instance = this;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onDisconnect += OnDisconnect;

        ResetVariables();
    }

    public void ResetVariables()
    {
        inGame = false;
        allTrue = false;
        readySpots = GameObject.Find("ReadySpots");
        readyAnims = readySpots.GetComponentsInChildren<Animator>();
        readyCubes = readySpots.GetComponent<ReadyCubes>();
        timerText = GameObject.Find("timerText").GetComponent<Text>();
        timerTextShadow = GameObject.Find("timerTextShadow").GetComponent<Text>();
    }

    void OnReady(string code)
    {
        //Initialize Game State
        JObject newGameState = new JObject();
        newGameState.Add("view", new JObject());
        newGameState.Add("playerColors", new JObject());

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
                AirConsole.instance.Message(device_id, "Menu");
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    if (GameObject.Find(i.ToString()) == null)
                    {
                        GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
                        newPlayer.name = i.ToString();
                    }
                }
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
                Player player = GameObject.Find("" + active_player).GetComponent<Player>();
                player.isReady = !player.isReady;

                if (player.isReady == true)
                {
                    AllPlayersReady(AirConsole.instance.GetControllerDeviceIds().Count);
                    readyAnims[active_player].SetBool("Joined", true);
                }
                else
                {
                    readyCubes.allReady = false;
                    readyAnims[active_player].SetBool("Joined", false);
                }

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
        int numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
        print("number of players: " + numberOfPlayers);
        AirConsole.instance.SetActivePlayers(numberOfPlayers);
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
        int numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
        print("number of players: " + numberOfPlayers);
        AirConsole.instance.SetActivePlayers(numberOfPlayers);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i == 0) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait1");
            else if (i == 1) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait2");
            else if (i == 2) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait3");
            else if (i == 3) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait4");
            else if (i == 4) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait5");
            else if (i == 5) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait6");
            else if (i == 6) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait7");
            else if (i == 7) AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Wait8");

        }
    }

    void AllPlayersReady(int numberOfPlayers)
    {
        allTrue = true;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<Player>().isReady == false)
            {
                allTrue = false;
                readyCubes.allReady = false;
                break;
            }
        }

        if (allTrue) StartCoroutine(AllReadyDelay()); //AND STOP LETTING PEOPLE IN??
    }

    IEnumerator AllReadyDelay()
    {
        readyCubes.allReady = true; //gobal variable, will be set false if any disconnect or player presses ready again
        for (int i = 3; i >= 0; i--) //while everybody is ready OR timer ends
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
        if (readyCubes.allReady)
        {
            inGame = true;
            SetWaitScreens();
            SceneManager.LoadScene("Game");
        }
        else Debug.Log("Not everyone is ready!");

    }
 
#endif
}
