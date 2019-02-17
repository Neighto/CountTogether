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
    public GameObject purblex1;
    public GameObject purblex2;
    public GameObject purblex3;
    public GameObject flyingPurble;
    public GameObject purpleBird;
    public GameObject roborg;
    public GameObject ghost;
    public GameObject fakeGreengo;
    public GameObject greengox1;
    public GameObject greengox2;
    public GameObject greengox3;
    public GameObject flyingGreengo;
    public GameObject doog;
    public GameObject cherb;
    public GameObject cherbPurple;
    public GameObject gruub;
    public GameObject bluebey;
    public GameObject batte;
    public GameObject fisk;

    //Sets
    private List<Set> easySets;
    private List<Set> hardSets;
    private Set countEverything;

    //Rounds UI
    public GameObject round;
    private List<RawImage> roundList;

    //Flavors UI
    public GameObject wildAndFast;
    public GameObject birdsOfAFeather;
    public GameObject blueConfusions;
    public GameObject countThemAll;
    public GameObject greengoSwarm;
    public GameObject greenerGood;
    public GameObject noKidsAllowed;
    public GameObject purbleParade;
    public GameObject purpleProblems;
    public GameObject purbleSwarm;
    public GameObject powerInNumbers;

    //Spawn Points
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> revSpawnPoints = new List<Transform>();
    public GameObject spawnPointsObj;
    public GameObject revSpawnPointObj;
    public Transform roborgSpawn;
    public Transform fiskSpawn;

    //Animations
    public GameObject purbleAnim;
    public GameObject greengoAnim;
    public GameObject bluebeyAnim;
    public GameObject batteAnim;
    public GameObject birdpleAnim;
    public GameObject cherbAnim;
    public GameObject doogAnim;
    public GameObject everything;
    public Animator startAnim;
    public Animator endAnim;

    //Timers
    private float spawnRateRandom = 1.5f;
    private float nextSpawnRandom = 0.0f;
    private float fishSpawnRate = 6f;
    private float nextFishSpawn = 0.0f;

    //All UI
    public AnimateUI animateUI;

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
    private bool fishSpawning = false;
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
        Set purbleSwarm = new Set("purble", purbleAnim, purbleParade, new GameObject[] { purblex1, purblex2, purblex3, flyingPurble, batte } );
        Set greengoSwarm = new Set("greengo", greengoAnim, greenerGood, new GameObject[] { greengox1, greengox2, greengox3, flyingGreengo, fakeGreengo, cherb });
        Set purpleConfusion = new Set("purble", purbleAnim, purpleProblems, new GameObject[] { purblex1, purblex2, purblex3, flyingPurble, flyingGreengo, purpleBird, cherbPurple });
        Set purpleConfusion2 = new Set("birdple", birdpleAnim, purpleProblems, new GameObject[] { purblex1, purblex2, purblex3, flyingPurble, purpleBird, purpleBird, purpleBird, cherbPurple });
        Set blueConfusion = new Set("bluebey", bluebeyAnim, blueConfusions, new GameObject[] { bluebey, bluebey, bluebey, gruub, batte, ghost });
        Set blueConfusion2 = new Set("batte", batteAnim, blueConfusions, new GameObject[] { bluebey, gruub, batte, batte, batte, ghost, ghost });
        Set birdsOfFeather = new Set("birdple", birdpleAnim, birdsOfAFeather, new GameObject[] { purpleBird, purpleBird, purpleBird, batte, cherb, cherbPurple, flyingPurble });
        Set birdsOfFeather2 = new Set("cherb", cherbAnim, birdsOfAFeather, new GameObject[] { purpleBird, batte, cherb, cherb, cherb, cherbPurple, flyingPurble });
        Set noPurbles = new Set("cherb", cherbAnim, noKidsAllowed, new GameObject[] { cherb, cherb, cherb, cherbPurple, doog, batte, purpleBird, ghost });
        Set noPurbles2 = new Set("doog", doogAnim, noKidsAllowed, new GameObject[] { cherb, cherbPurple, doog, doog, doog, doog, batte, purpleBird, ghost });
        Set weirdos = new Set("doog", doogAnim, wildAndFast, new GameObject[] { doog, doog, doog, doog, gruub, roborg });
        Set allPurbles = new Set("purble", purbleAnim, powerInNumbers, new GameObject[] { purblex1, purblex2, purblex3, purblex1, purblex2, purblex3, flyingPurble, bluebey, greengox1, greengox2, greengox3 });
        Set allPurbles2 = new Set("greengo", greengoAnim, powerInNumbers, new GameObject[] { purblex1, purblex2, purblex3, flyingPurble, bluebey, greengox1, greengox2, greengox3, greengox1, greengox2, greengox3 });
        Set allPurbles3 = new Set("bluebey", bluebeyAnim, powerInNumbers, new GameObject[] { purblex1, purblex2, flyingPurble, bluebey, bluebey, bluebey, bluebey, bluebey, bluebey, greengox1, greengox2 });
        easySets = new List<Set> { purbleSwarm, blueConfusion, blueConfusion2, greengoSwarm, purpleConfusion };
        hardSets = new List<Set> { birdsOfFeather, birdsOfFeather2, noPurbles, noPurbles2, allPurbles, allPurbles2, allPurbles3, weirdos };
        countEverything = new Set("all", everything, countThemAll, new GameObject[] { purblex1, purblex2, purblex3, greengox2, greengox3, bluebey,
        purpleBird, gruub, fakeGreengo, cherb, cherbPurple, doog, ghost, batte, flyingGreengo});

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

        //Find Best Score
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            if (p.score > best)
            {
                best = p.score;
                winner = p;
            }
        }

        //Find Ties
        bool once = false;
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            
            if (p.score == best)
            {
                if (once)
                {
                    gameTied = true;
                    break;
                }
                once = true;
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
                spawnRateRandom = 0.3f * spawnRateRandom; //increase randoms spawn rate
                curSet = GetEasySet();
                fishSpawning = true;
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
                spawnRateRandom = 2f * spawnRateRandom; //increase randoms spawn rate
                fishSpawnRate = 0.5f * fishSpawnRate; //increase fish spawn rate
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
        yield return new WaitForSeconds(1f); // HIDE ROUND START TEXT / SHOW REMINDER PANEL / COUNTING ENABLED / SPAWNING ENABLED
        animateUI.ShiftDarkenBackground(); //Lighten
        yield return new WaitForSeconds(1f);
        if (gameLogic != null) gameLogic.SetCountScreens();
        canSpawn = true;
        if (currentRound == 5)
        {
            Instantiate(roborg, roborgSpawn.position, roborgSpawn.rotation);
            monsterSum++;
        }
        yield return new WaitForSeconds(10f); // SPAWNING DISABLED AFTER X SECONDS
        canSpawn = false;
        yield return new WaitForSeconds(8f); // SHOW ROUND END TEXT / DISABLE COUNTING / HIDE REMINDER PANEL
        if (gameLogic != null) gameLogic.SetWaitScreens();
        endAnim.SetTrigger("End");
        GetPlayerEstimates();
        yield return new WaitForSeconds(0.5f);
        animateUI.ShiftDarkenBackground(); //Darken
        yield return new WaitForSeconds(1.5f); // SHOW RESULTS PANEL AND PLAYER ESTIMATES / HIDE ROUND END TEXT
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
            int n = int.Parse(winner.name) + 1;
            winnerName.text = "PLAYER " + n;
        }
        yield return new WaitForSeconds(6f);
        if (gameLogic != null) gameLogic.SetMenuScreens();
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
            else if (fishSpawning && Time.time > nextFishSpawn)
            {
                nextFishSpawn = Time.time + fishSpawnRate;
                Instantiate(fisk, fiskSpawn.position, fiskSpawn.rotation);
                if (currentRound == 5) monsterSum++;
            }

        }
    }

}
