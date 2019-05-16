using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

    //Music
    public AudioSource music;

    //SFX
    private AudioSource source;
    public AudioClip[] pops;
    public AudioClip[] whistles;

    //Rounds UI
    public GameObject startGame;
    public GameObject[] instructionTexts;
    public List<GameObject> roundList;

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
    List<GameObject> players;
    private int numberOfPlayers;
    public GameObject playersPanel;
    public DynamicPlayerNames dynamicPlayerNames;
    public GameObject playerContext;
    private Text playerContextText;
    private Animator contextAnimator;
    public Text roundText;
    public Text answerText;

    //Winner UI
    public GameObject winnerPanel;
    List<Tuple<int, int>> winners = new List<Tuple<int, int>>();
    public GameObject winnerName;
    private Text winnerNameText;
    private Animator winnerNameAnimator;
    public Text winnerText;

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
    private Set curSet = null;
    private Set prevSet = null;

    private void Start()
    {
        //Make Lists
        foreach (Transform t in spawnPointsObj.transform)
            spawnPoints.Add(t);
        foreach (Transform t in revSpawnPointObj.transform)
            revSpawnPoints.Add(t);

        //Get AudioSource
        source = this.GetComponent<AudioSource>();

        //Get text and animators
        playerContextText = playerContext.GetComponent<Text>();
        contextAnimator = playerContext.GetComponent<Animator>();
        winnerNameText = winnerName.GetComponent<Text>();
        winnerNameAnimator = winnerName.GetComponent<Animator>();

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
        countEverything = new Set("everything", everything, countThemAll, new GameObject[] { purblex1, purblex2, purblex3, greengox2, greengox3, bluebey,
        purpleBird, gruub, fakeGreengo, cherb, cherbPurple, doog, ghost, batte, flyingGreengo});

        //Find GameLogic for SetView access
        if (GameObject.Find("GameLogic"))
        {
            gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
            numberOfPlayers = gameLogic.numberOfPlayers;
        }

        //Get players
        players = new List<GameObject>();
        players.Add(GameObject.Find("0"));
        players.Add(GameObject.Find("1"));
        players.Add(GameObject.Find("2"));
        players.Add(GameObject.Find("3"));
        players.Add(GameObject.Find("4"));
        players.Add(GameObject.Find("5"));
        players.Add(GameObject.Find("6"));
        players.Add(GameObject.Find("7"));

        StartCoroutine(InstructionDisplay()); //this calls setround
    }

    //Get an easy set
    Set GetEasySet()
    {
        Set set = easySets[UnityEngine.Random.Range(0, easySets.Count)];
        while (set == prevSet)
        {
            set = easySets[UnityEngine.Random.Range(0, easySets.Count)];
        }
        prevSet = set;
        return set;
    }

    //Get a hard set
    Set GetHardSet()
    {
        Set set = hardSets[UnityEngine.Random.Range(0, hardSets.Count)];
        while (set == prevSet)
        {
            set = hardSets[UnityEngine.Random.Range(0, hardSets.Count)];
        }
        prevSet = set;
        return set;
    }

    //Get random monster from a given set
    GameObject GetRandomMonster(GameObject[] monsters)
    {
        return monsters[UnityEngine.Random.Range(0, monsters.Length)];
    }

    //Where monsters spawn (add diversity)
    Transform GetRandomSpawn()
    {
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
    }

    Transform GetReverseRandomSpawn()
    {
        return revSpawnPoints[UnityEngine.Random.Range(0, revSpawnPoints.Count)];
    }

    //Get player estimates from each player prefab in the game
    void GetPlayerEstimates()
    {
        playerContextText.text = "WHAT DID YOU GUESS?";
        for (int i = 0; i < numberOfPlayers; i++) //might cause an array error
        {
            Player p = players[i].GetComponent<Player>();
            dynamicPlayerNames.playerCountTexts[i].text = p.playerCount.ToString();
            p.roundScore = (int)Mathf.Max(0, points - Mathf.Pow(Mathf.Abs(p.playerCount - monsterSum), 2));
            p.playerCount = 0; //Reset
        }
    }

    void GetPlayerScores()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Player p = players[i].GetComponent<Player>();
            dynamicPlayerNames.playerCountTexts[i].text = p.score.ToString();
            if (p.roundScore > 0)
            {
                dynamicPlayerNames.playerPointTexts[i].text = "+" + p.roundScore.ToString(); //should be just points of round
                dynamicPlayerNames.playerAnimators[i].SetTrigger("Add");
                p.score += p.roundScore;
            }
        }
        StartCoroutine(DelayAddition());

    }

    IEnumerator DelayAddition()
    {
        yield return new WaitForSeconds(1.05f);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Player p = players[i].GetComponent<Player>();
            dynamicPlayerNames.playerCountTexts[i].text = p.score.ToString();
        }
    }

    //Compare everybody's scores
    void WhoWon() //get playerID name, if tie, crown multiple winners (up to 8)
    {
        //Sort scores
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Player p = players[i].GetComponent<Player>();
            winners.Add(new Tuple<int, int>(p.score, i));
        }
        winners.Sort((x, y) => y.Item1.CompareTo(x.Item1));

        int j = 1;
        for (int i = 1; i < numberOfPlayers; i++)
        {
            if (winners[j].Item1 != winners[j - 1].Item1)
            {
                winners.RemoveAt(j); 
            }
            else
            {
                j++;
            }
        }
        winners.TrimExcess();
        if (winners.Count > 1) winnerText.text = "AND THE WINNERS ARE";
        else winnerText.text = "AND THE WINNER IS";
    }

    //Sets up UI for each round (panels, pictures, names, flavortext, etc)
    void SetAllUI(bool turnOn)
    {
        //Set-up UI
        roundText.text = "ROUND " + currentRound;
        roundList[currentRound - 1].SetActive(turnOn);
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
                spawnRateRandom = 0.8f * spawnRateRandom; //increase randoms spawn rate
                points = 40;
            }
            else if (roundNumber == 5) //Count everything
            {
                curSet = countEverything;
                spawnRateRandom = 2f * spawnRateRandom; //increase randoms spawn rate
                fishSpawnRate = 0.5f * fishSpawnRate; //increase fish spawn rate
                points = 50;
            }
            //if curset.name == greengo {gameLogic.setgreengoscreens}
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
        if (gameLogic != null) SetupPhonesForRound(curSet.name);
        canSpawn = true;
        if (currentRound == 5)
        {
            Instantiate(roborg, roborgSpawn.position, roborgSpawn.rotation);
            monsterSum++;
            yield return new WaitForSeconds(3f);
        }
        if (currentRound < 2) yield return new WaitForSeconds(6f);
        else yield return new WaitForSeconds(8f); // SPAWNING DISABLED AFTER X SECONDS
        canSpawn = false;
        yield return new WaitForSeconds(9f); // SHOW ROUND END TEXT / DISABLE COUNTING / HIDE REMINDER PANEL
        if (gameLogic != null) gameLogic.SetWaitScreens(true);
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
        yield return new WaitForSeconds(1.8f); // AWARD POINTS TO PLAYERS
        contextAnimator.SetTrigger("Switch");
        yield return new WaitForSeconds(0.2f);
        playerContextText.text = "AWARDING POINTS...";
        GetPlayerScores();
        yield return new WaitForSeconds(1.2f);
        contextAnimator.SetTrigger("Switch");
        yield return new WaitForSeconds(0.2f);
        playerContextText.text = "PLAYER SCORES";
        yield return new WaitForSeconds(2f); // HIDE PLAYER PANEL AND RESULTS PANEL
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

        for (int i = 0; i < winners.Count; i++)
        {
            int n = winners[i].Item2;
            winnerNameText.text = dynamicPlayerNames.playerNameTexts[n].text;
            winnerNameAnimator.SetTrigger("Bounce");
            yield return new WaitForSeconds(1.66f);
        }

        winnerText.text = "";
        yield return new WaitForSeconds(1f);
        if (gameLogic != null) gameLogic.SetMenuScreens();
        SceneManager.LoadScene("Lobby");
    }

    //Initial instructions
    IEnumerator InstructionDisplay()
    {
        yield return new WaitForSeconds(0.5f);
        instructionTexts[0].SetActive(true);
        source.PlayOneShot(pops[0]);
        yield return new WaitForSeconds(1.3f);
        instructionTexts[1].SetActive(true);
        source.PlayOneShot(pops[1]);
        yield return new WaitForSeconds(1.3f);
        instructionTexts[2].SetActive(true);
        source.PlayOneShot(pops[2]);
        yield return new WaitForSeconds(0.8f);
        source.PlayOneShot(whistles[0]);
        yield return new WaitForSeconds(1.2f);
        startGame.SetActive(false);
        music.Play();
        dynamicPlayerNames.Setup(numberOfPlayers);
        if (gameLogic != null) gameLogic.SetNames(dynamicPlayerNames.playerNameTexts);
        SetRound(currentRound); //initialized to 1
    }

    void SetupPhonesForRound(string creatureName)
    {
        gameLogic.SetCountScreens();
        gameLogic.SetCreaturesOnScreens("_" + creatureName);
    }

    //Update for instantiating monsters in a timely fashion
    void Update()
    {
        if (canSpawn)
        {
            if (Time.time > nextSpawnRandom) //SPAWN RANDOM CREATURES
            {
                nextSpawnRandom = Time.time + spawnRateRandom + UnityEngine.Random.Range(-0.2f, 0.2f);
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
