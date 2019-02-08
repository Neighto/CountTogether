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

    private string[] colorNames = new string[] { "red", "blue", "green", "yellow", "orange", "purple", "pink" };
    private int colorIndex;
    private bool allTrue = false;

    public GameObject player;
    public Text uiText;
#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        readyCubes = readyCubesObj.GetComponentsInChildren<Renderer>();
        readyCubesScript = readyCubesObj.GetComponent<ReadyCubes>();
    }

    void OnConnect(int device_id)
    {
        //Initialize Game State
        JObject newGameState = new JObject();
        newGameState.Add("view", new JObject());
        newGameState.Add("playerColors", new JObject());
        AirConsole.instance.SetCustomDeviceState(newGameState);

        //UI
        if (AirConsole.instance.GetActivePlayerDeviceIds.Count > 2)
        {
            //Do not make Player for them

        }
        else
        {
            StartGame(); //this will be called by every player each time (bad practice)
        }

    }

    void OnDisconnect(int device_id)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        readyCubes[active_player].material.color = Color.white;
        readyCubesScript.allReady = false;
        Destroy(GameObject.Find("" + active_player));

        if (active_player != -1)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2 && AirConsole.instance.GetControllerDeviceIds().Count <= 4)
            {
            }
            else
            {
                AirConsole.instance.SetActivePlayers(0);
                //uiText.text = "PLEASE CONNECT 2-4 PLAYERS";
            }
        }
    }

    void OnMessage(int device_id, JToken data)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        string action = (string)data["action"];
        if (action != null)
        {
            if (action == "count")
            {
                Player player = GameObject.Find("" + active_player).GetComponent<Player>();
                player.AddCount();
            }

            if (action == "prepped")
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

    void StartGame() //if you enter as player 3, never stop being player 3, a NEW player will take the place of disconnected
    {
        int numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
        AirConsole.instance.SetActivePlayers(numberOfPlayers);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (GameObject.Find("" + i) == null)
            {
                GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
                newPlayer.name = "" + i;
            }
        }
    }

    void OnDestroy()
    {
        // unregister airconsole events on scene change
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    public void SetView(string viewName)
    {
        //I don't need to replace the entire game state, I can just set the view property
        AirConsole.instance.SetCustomDeviceStateProperty("view", viewName);
        //the controller listens for the onCustomDeviceStateChanged event. See the  controller-gamestates.html file for how this is handled there. 
    }

    void AllPlayersReady(int numberOfPlayers)
    {
        allTrue = true;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (GameObject.Find("" + i) != null)
            {
                if (GameObject.Find("" + i).GetComponent<Player>().isReady == false)
                {
                    allTrue = false;
                    readyCubesScript.allReady = false;
                    break;
                }
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
                uiText.text = "PLAYERS";
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

    /*
    public void AssignPlayerColors()
    {
        if (!AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            Debug.LogWarning("can't assign player colors until plugin is ready");
            return;
        }

        //make a copy of connected controller IDs so it can't change while I loop through it
        List<int> controllerIDs = AirConsole.instance.GetControllerDeviceIds();

        //loop through connected devices
        for (int i = 0; i < controllerIDs.Count; ++i)
        {
            //ideally, you'd write all the data into the game state first and then set it only once. 
            //I'm doing it this way for simplicity, but updating the device state too often can mean your updates get delayed because of rate limiting
            //the more devices are connected, the more this becomes a problem
            AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), controllerIDs[i], colorNames[colorIndex]));
            //the controller listens for the onCustomDeviceStateChanged event. See the  controller-gamestates.html file for how this is handled there. 

            //different color for the next player
            colorIndex++;
            if (colorIndex == colorNames.Length)
            {
                colorIndex = 0;
            }
        }
    }

    public static JToken UpdatePlayerColorData(JToken oldGameState, int deviceId, string colorName)
    {

        //take out the existing playerColorData and store it as a JObject so I can modify it
        JObject playerColorData = oldGameState["playerColors"] as JObject;

        //check if the playerColorData object within the game state already has data for this device
        if (playerColorData.HasValues && playerColorData[deviceId.ToString()] != null)
        {
            //there is already color data for this device, replace it
            playerColorData[deviceId.ToString()] = colorName;
        }
        else
        {
            playerColorData.Add(deviceId.ToString(), colorName);
            //there is no color data for this device yet, create it new
        }

        //logging and returning the updated playerColorData
        Debug.Log("AssignPlayerColor for device " + deviceId + " returning new playerColorData: " + playerColorData);
        return playerColorData;
    }
    */
#endif
}
