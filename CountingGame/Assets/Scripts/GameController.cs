using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    //DEFINED SET CLASS
    public class Set
    {
        public string name;
        public GameObject biasMonsterAnim;
        public GameObject flavorText;
        public GameObject[] monsters;


        public Set(string name, GameObject biasMonsterAnim, 
            GameObject flavorText, GameObject[] monsters)
        {
            this.name = name; //tag of what you are counting
            this.biasMonsterAnim = biasMonsterAnim;
            this.flavorText = flavorText;
            this.monsters = monsters;
        }
    }

    //Monsters
    private List<GameObject> monsters;
    public GameObject purpleBallx1;
    public GameObject purpleBallx2;
    public GameObject purpleBallx3;
    public GameObject purpleBird;
    public GameObject roborg;
    public GameObject ghost;
    public GameObject fakeGreengo;
    public GameObject greengox2;
    public GameObject greengox3;
    public GameObject doog;
    public GameObject cherb;
    public GameObject cherpPurple;
    public GameObject gruub;
    public GameObject bluebey;

    //Sets
    private List<Set> easySets;
    private List<Set> hardSets;
    private Set countEverything;

    //Rounds UI
    public GameObject round;
    private List<RawImage> roundList;

    //Flavors UI
    public GameObject purbleSwarmFlavor;

    //Spawn Points
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> revSpawnPoints = new List<Transform>();
    public GameObject spawnPointsObj;
    public GameObject revSpawnPointObj;
    public Transform roborgSpawn;

    //Animations
    public GameObject purbleAnim;
    public Animator startAnim;

    //Timers
    private float spawnRateRandom = 1.5f;
    private float nextSpawnRandom = 0.0f;

    //All UI
    public AnimateUI animateUI;

    //Results UI
    /*
    public GameObject resultsPanel;
    public Text youWereCountingText;
    public RawImage resMonsterPic;*/

    //Players UI
    private List<Text> playerCountTexts;
    List<GameObject> players;
    public GameObject playersPanel;
    public Text countText1;
    public Text countText2;
    public Text countText3;
    public Text countText4;
    public Text countText5;
    public Text countText6;
    public Text countText7;
    public Text countText8;
    public Text playerContextText;
    public Text roundText;
    public Text answerText;

    //Winner UI
    public GameObject winnerPanel;
    public Text winnerName;

    //Other
    private bool canSpawn = false;
    private bool reverseMonsters = false;
    private bool gameTied = false;
    private int monsterSum = 0;
    private int currentRound = 1;
    private int points = 10;
    private GameLogic gameLogic;
    private Transform ranTran;
    private Transform revRanTran;
    private Set curSet;
    Player winner;

    private void Start()
    {
        //Make Lists
        roundList = new List<RawImage>(round.GetComponentsInChildren<RawImage>());
        playerCountTexts = new List<Text> { countText1, countText2, countText3, countText4, countText5, countText6, countText7, countText8 };
        foreach (Transform t in spawnPointsObj.transform)
            spawnPoints.Add(t);
        foreach (Transform t in revSpawnPointObj.transform)
            revSpawnPoints.Add(t);

        //Make Sets
        Set purbleSwarm = new Set("purble", purbleAnim, purbleSwarmFlavor, new GameObject[] { purpleBallx1, purpleBallx2, purpleBallx3 } );
        Set greenConfusion = new Set("greengo", purbleAnim, purbleSwarmFlavor, new GameObject[] { cherb, fakeGreengo, greengox2, greengox3, greengox2, ghost });
        Set purpleConfusion = new Set("purble", purbleAnim, purbleSwarmFlavor, new GameObject[] { purpleBallx1, purpleBallx2, purpleBallx3, purpleBird, cherpPurple });
        Set birdsOfFeather = new Set("bird", purbleAnim, purbleSwarmFlavor, new GameObject[] { purpleBird, doog });
        Set blueConfusion = new Set("bluebey", purbleAnim, purbleSwarmFlavor, new GameObject[] { bluebey, gruub });
        easySets = new List<Set> { purbleSwarm, blueConfusion, greenConfusion, purpleConfusion };
        hardSets = new List<Set> { birdsOfFeather };
        countEverything = new Set("all", purbleAnim, purbleSwarmFlavor, new GameObject[] { purpleBallx1, purpleBallx2, purpleBallx3, greengox2, greengox3, bluebey,
        purpleBird, gruub, fakeGreengo, cherb, cherpPurple, doog, ghost});

        //Find GameLogic for SetView access
        if (GameObject.Find("GameLogic")) gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();

        SetRound(currentRound); //initialized to 1
    }

    //Get an easy set
    Set GetEasySet()
    {
        return easySets[Random.Range(0, easySets.Count)];
    }

    //Get a hard set
    Set GetHardSet()
    {
        return hardSets[Random.Range(0, hardSets.Count)];
    }

    //Get random monster from a given set
    GameObject GetRandomMonster(GameObject[] monsters)
    {
        return monsters[Random.Range(0, monsters.Length)];
    }

    //Where monsters spawn (add diversity)
    Transform GetRandomSpawn()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    Transform GetReverseRandomSpawn()
    {
        return revSpawnPoints[Random.Range(0, revSpawnPoints.Count)];
    }

    //Get player estimates from each player prefab in the game
    void GetPlayerEstimates()
    {
        playerContextText.text = "WHAT DID YOU GUESS?";
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            playerCountTexts[i].text = p.GetPlayerCount().ToString();
            p.score += (int)Mathf.Max(0, points - Mathf.Pow(Mathf.Abs(p.GetPlayerCount() - monsterSum), 2));
            p.ResetPlayerCount();
        }
    }

    void GetPlayerScores()
    {
        playerContextText.text = "PLAYER SCORES";
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            playerCountTexts[i].text = p.score.ToString();
        }
    }

    //Compare everybody's scores
    void WhoWon()
    {
        int best = -1;
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            if (p.score > best)
            {
                best = p.score;
                winner = p;
            }
            else if (p.score == best)
            {
                gameTied = true;
                break;
            }
        }
    }

    //Sets up UI for each round (panels, pictures, names, flavortext, etc)
    void SetAllUI(bool turnOn)
    {
        //Set-up UI
        roundText.text = "ROUND " + currentRound;
        roundList[currentRound - 1].enabled = turnOn;
        curSet.flavorText.SetActive(turnOn);
        curSet.biasMonsterAnim.SetActive(turnOn);

        if (currentRound == 5)
        {

            //flavorText.text = "The ultimate test!";
        }
    }

    //Handles start of each round (function calls and difficulty)
    void SetRound(int roundNumber)
    {
        monsterSum = 0;

        if (roundNumber < 6) //number of rounds
        {
            if (roundNumber == 1)
            {
                curSet = GetEasySet();
            }
            else if (roundNumber == 2)
            {
                curSet = GetEasySet();
                spawnRateRandom = 0.6f * spawnRateRandom; //increase randoms spawn rate
                points = 20;
            }
            else if (roundNumber == 3)
            {
                curSet = GetHardSet();
                reverseMonsters = true;
                points = 30;
            }
            else if (roundNumber == 4)
            {
                curSet = GetHardSet();
                spawnRateRandom = 0.6f * spawnRateRandom; //increase randoms spawn rate
                points = 40;
            }
            else if (roundNumber == 5) //Count everything
            {
                curSet = countEverything;
                points = 50;
            }
            StartCoroutine(StartRound());
        }
        else //when all rounds are over:
        {
            StartCoroutine(EndGame());
        }
    }

    //Handles critical timing of UI and when to spawn
    IEnumerator StartRound()
    {
        SetAllUI(true);
        yield return new WaitForSeconds(3f); // HIDE ROUND PANEL / SHOW ROUND START TEXT
        SetAllUI(false);
        startAnim.SetTrigger("Start");
        yield return new WaitForSeconds(2f); // HIDE ROUND START TEXT / SHOW REMINDER PANEL / COUNTING ENABLED / SPAWNING ENABLED
        if (gameLogic != null) gameLogic.SetCountScreens();
        canSpawn = true;
        Instantiate(roborg, roborgSpawn.position, roborgSpawn.rotation);
        yield return new WaitForSeconds(10f); // SPAWNING DISABLED AFTER X SECONDS
        canSpawn = false;
        yield return new WaitForSeconds(8f); // SHOW ROUND END TEXT / DISABLE COUNTING / HIDE REMINDER PANEL
        //if (gameLogic != null) gameLogic.SetView("Wait");
        if (gameLogic != null) gameLogic.SetWaitScreens();
        //startEndText.text = "FINISH!";
        GetPlayerEstimates();
        yield return new WaitForSeconds(2f); // SHOW RESULTS PANEL AND PLAYER ESTIMATES / HIDE ROUND END TEXT
        animateUI.ShiftPlayersPanel();
        answerText.text = "THERE WERE";
        yield return new WaitForSeconds(0.5f); // SHOW MONSTER SUM
        answerText.text += ".";
        yield return new WaitForSeconds(0.5f); // SHOW MONSTER SUM
        answerText.text += ".";
        yield return new WaitForSeconds(0.5f); // SHOW MONSTER SUM
        answerText.text += ". ";
        yield return new WaitForSeconds(0.5f); // SHOW MONSTER SUM
        answerText.text = "THERE WERE..." + monsterSum;
        yield return new WaitForSeconds(2f); // AWARD POINTS TO PLAYERS
        GetPlayerScores();
        yield return new WaitForSeconds(1.4f); // HIDE PLAYER PANEL AND RESULTS PANEL
        animateUI.ShiftPlayersPanel();
        yield return new WaitForSeconds(1.5f); // PREPARE FOR NEW ROUND
        SetRound(++currentRound);
    }

    //Show the winner and exit scene (does not account for ties!)
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        winnerPanel.SetActive(true);
        WhoWon();
        yield return new WaitForSeconds(2f);
        if (gameTied || winner == null)
        {
            winnerName.text = "IT'S A TIE";
        }
        else
        {
            winnerName.text = "PLAYER " + (winner.name + 1);
        }
        yield return new WaitForSeconds(6f);
        //if (gameLogic != null) gameLogic.SetView("Menu");
        SceneManager.LoadScene("Lobby");
    }

    //Update for instantiating monsters in a timely fashion
    void Update()
    {
        if (canSpawn)
        {
            if (Time.time > nextSpawnRandom) //SPAWN RANDOM CREATURES
            {
                nextSpawnRandom = Time.time + spawnRateRandom;
                ranTran = GetRandomSpawn();
                GameObject mon = Instantiate(GetRandomMonster(curSet.monsters), ranTran.position, ranTran.rotation);
                if (mon.tag.Equals(curSet.name) || currentRound == 5)
                {
                    monsterSum += mon.GetComponent<MonsterStats>().quantity;
                }
                if (reverseMonsters)
                {
                    revRanTran = GetReverseRandomSpawn();
                    GameObject revMon = Instantiate(GetRandomMonster(curSet.monsters), revRanTran.position, revRanTran.rotation);
                    revMon.GetComponent<MonsterStats>().ChangeDirection();
                    if (revMon.tag.Equals(curSet.name) || currentRound == 5)
                    {
                        monsterSum += revMon.GetComponent<MonsterStats>().quantity;
                    }
                }

            }
        }
    }

}
