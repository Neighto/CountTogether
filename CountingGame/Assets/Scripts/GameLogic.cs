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
    public Text uiText;
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


    public void SetView(string viewName)
    {
        foreach (int id in AirConsole.instance.GetActivePlayerDeviceIds)
        {
            AirConsole.instance.Message(id, viewName);
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
        for (int i = 8; i > 0; i--) //while everybody is ready OR timer ends
        {
            uiText.text = "START IN " + i;
            yield return new WaitForSeconds(0.5f);
            if (readyCubesScript.allReady == false)
            {
                uiText.text = "COUNT TOGETHER";
                break;
            }
        }
        if (readyCubesScript.allReady)
        {
            SetView("Wait"); //set all ready players in game scene!
            SceneManager.LoadScene("Game");
        }
        else Debug.Log("Not everyone is ready!");

    }
 
#endif
}
