using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    public GameObject readyCubesObj;
    private ReadyCubes readyCubesScript;
    private Renderer[] readyCubes;
    private bool allTrue = false;

    public GameObject player;
    public Text timerText;
    public Text timerTextShadow;
#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onDisconnect += OnDisconnect;
        readyCubes = readyCubesObj.GetComponentsInChildren<Renderer>();
        readyCubesScript = readyCubesObj.GetComponent<ReadyCubes>();
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

    void OnDisconnect(int device_id)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        readyCubes[active_player].material.color = Color.white;
        readyCubesScript.allReady = false;
        //readyCubes[active_player].GetComponent<Player>().isReady = false;
        Destroy(GameObject.Find(active_player.ToString())); //maybe rem

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
                    readyCubes[active_player].material.color = Color.green;
                    AllPlayersReady(AirConsole.instance.GetControllerDeviceIds().Count);

                }
                else
                {
                    readyCubesScript.allReady = false;
                    readyCubes[active_player].material.color = Color.white;
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
                readyCubesScript.allReady = false;
                break;
            }
        }

        if (allTrue) StartCoroutine(AllReadyDelay()); //AND STOP LETTING PEOPLE IN??
    }

    IEnumerator AllReadyDelay()
    {
        readyCubesScript.allReady = true; //gobal variable, will be set false if any disconnect or player presses ready again
        for (int i = 3; i >= 0; i--) //while everybody is ready OR timer ends
        {
            timerText.text = "START IN " + i;
            timerTextShadow.text = "START IN " + i;
            yield return new WaitForSeconds(1f);
            if (readyCubesScript.allReady == false)
            {
                timerText.text = "";
                timerTextShadow.text = "";
                break;
            }
        }
        if (readyCubesScript.allReady)
        {
            //SetView("Wait"); //set all ready players in game scene!
            SetWaitScreens();
            SceneManager.LoadScene("Game");
        }
        else Debug.Log("Not everyone is ready!");

    }
 
#endif
}
