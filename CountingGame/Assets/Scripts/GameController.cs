using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    //DEFINED MONSTER CLASS
    public class Monster
    {
        public string name;
        public string flavorText;
        public Texture monsterPic;
        public GameObject monster;

        public Monster(string name, string flavorText, Texture monsterPic, GameObject monster)
        {
            this.name = name;
            this.flavorText = flavorText;
            this.monsterPic = monsterPic;
            this.monster = monster;
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

    //Defined Monsters
    private List<Monster> definedMonsters;
    Monster curMon;

    //Textures
    public Texture picPurpleBall;
    public Texture picPurpleBird;

    //Spawn Points
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> revSpawnPoints = new List<Transform>();
    public GameObject spawnPointsObj;
    public GameObject revSpawnPointObj;

    //Timers
    private float spawnRateRandom = 2.0f;
    private float nextSpawnRandom = 0.0f;
    private float spawnRateBiased = 1.5f;
    private float nextSpawnBiased = 0.0f;

    //All UI
    public AnimateUI animateUI;

    //Round UI
    public GameObject roundPanel;
    public Text roundText;
    public Text flavorText;
    public RawImage rouMonsterPic;
    public Text startEndText;

    //Results UI
    public GameObject resultsPanel;
    public Text roundText2;
    public Text youWereCountingText;
    public Text answerText;
    public RawImage resMonsterPic;

    //Players UI
    private List<Text> playerCountTexts;
    List<GameObject> players;
    public GameObject playersPanel;
    public Text countText1;
    public Text countText2;
    public Text playerContextText;

    //Reminder UI
    public GameObject reminderPanel;
    public RawImage remMonsterPic;

    //Other
    private bool canSpawn = false;
    private bool reverseMonsters = false;
    private int monsterSum = 0;
    private int currentRound = 1;
    private GameLogic gameLogic;
    private Transform ranTran;
    private Transform revRanTran;

    private void Start()
    {
        //Define Monsters
        Monster birdple = new Monster("birdple", "Wild and Fast.", picPurpleBird, purpleBird);
        Monster purble = new Monster("purble", "Careful, they bite!", picPurpleBall, purpleBallx1);

        //Make Lists
        monsters = new List<GameObject>
        { purpleBird, purpleBallx1, purpleBallx2, purpleBallx3, roborg, ghost, fakeGreengo, greengox2, greengox3 };
        definedMonsters = new List<Monster> { birdple, purble };
        playerCountTexts = new List<Text> { countText1, countText2 };
        foreach (Transform t in spawnPointsObj.transform)
            spawnPoints.Add(t);
        foreach (Transform t in revSpawnPointObj.transform)
            revSpawnPoints.Add(t);

        //Find GameLogic for SetView access
        if (GameObject.Find("GameLogic"))
        {
            gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        }

        SetRound(currentRound); //initialized to 1
    }

    //Defined monster: One designed for counting in a round
    Monster GetDefinedMonster()
    {
        return definedMonsters[Random.Range(0, definedMonsters.Count)];
    }

    //Undefined monster: All monsters in the game
    GameObject GetRandomMonster()
    {
        return monsters[Random.Range(0, monsters.Count)];
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
            p.score += CalculateScore(p.GetPlayerCount(), monsterSum);
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

    //Sets up UI for each round (panels, pictures, names, flavortext, etc)
    void SetAllUI(int roundNumber, Monster curMon)
    {
        //Set-up UI
        roundText.text = "ROUND " + roundNumber;
        roundText2.text = "ROUND " + roundNumber;
        startEndText.text = "START COUNTING!";
        flavorText.text = curMon.flavorText;
        rouMonsterPic.texture = curMon.monsterPic;
        resMonsterPic.texture = curMon.monsterPic;
        remMonsterPic.texture = curMon.monsterPic;
    }

    //Handles start of each round (function calls and difficulty)
    void SetRound(int roundNumber)
    {
        monsterSum = 0;

        if (roundNumber < 6) //number of rounds
        {
            if (roundNumber == 1)
            {

            }
            else if (roundNumber == 2)
            {
                spawnRateRandom = 0.6f * spawnRateRandom; //increase randoms spawn rate
            }
            else if (roundNumber == 3)
            {
                reverseMonsters = true;
            }
            else if (roundNumber == 4)
            {

            }
            else if (roundNumber == 5)
            {

            }
            curMon = GetDefinedMonster();
            SetAllUI(roundNumber, curMon);
            StartCoroutine(StartRound());
        }
        else //when all rounds are over:
        {
            if (gameLogic != null) gameLogic.SetView("Menu");
            SceneManager.LoadScene("Lobby");
        }
    }

    //Handles critical timing of UI and when to spawn
    IEnumerator StartRound()
    {
        roundPanel.SetActive(true);
        yield return new WaitForSeconds(3f); // HIDE ROUND PANEL / SHOW ROUND START TEXT
        roundPanel.SetActive(false);
        startEndText.enabled = true;
        yield return new WaitForSeconds(2f); // HIDE ROUND START TEXT / SHOW REMINDER PANEL / COUNTING ENABLED / SPAWNING ENABLED
        if (gameLogic != null) gameLogic.SetView("Count");
        startEndText.enabled = false;
        animateUI.ShiftReminderPanel();
        canSpawn = true;
        yield return new WaitForSeconds(10f); // SPAWNING DISABLED AFTER X SECONDS
        canSpawn = false;
        yield return new WaitForSeconds(8f); // SHOW ROUND END TEXT / DISABLE COUNTING / HIDE REMINDER PANEL
        if (gameLogic != null) gameLogic.SetView("Wait");
        startEndText.text = "FINISH!";
        startEndText.enabled = true;
        animateUI.ShiftReminderPanel();
        GetPlayerEstimates();
        yield return new WaitForSeconds(2f); // SHOW RESULTS PANEL AND PLAYER ESTIMATES / HIDE ROUND END TEXT
        answerText.text = "THERE WERE...\n";
        animateUI.ShiftPlayersPanel();
        startEndText.enabled = false;
        resultsPanel.SetActive(true);
        yield return new WaitForSeconds(2.4f); // SHOW MONSTER SUM
        answerText.text = "THERE WERE...\n" + monsterSum;
        yield return new WaitForSeconds(2f); // AWARD POINTS TO PLAYERS
        GetPlayerScores();
        yield return new WaitForSeconds(1f); // HIDE PLAYER PANEL AND RESULTS PANEL
        animateUI.ShiftPlayersPanel();
        resultsPanel.SetActive(false);
        yield return new WaitForSeconds(1.5f); // PREPARE FOR NEW ROUND
        SetRound(++currentRound);
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
                GameObject mon = Instantiate(GetRandomMonster(), ranTran.position, ranTran.rotation);
                if (mon.tag.Equals(curMon.name))
                {
                    monsterSum += mon.GetComponent<MonsterStats>().quantity;
                }
                Destroy(mon, 15f);
                if (reverseMonsters)
                {
                    revRanTran = GetReverseRandomSpawn();
                    GameObject revMon = Instantiate(GetRandomMonster(), revRanTran.position, revRanTran.rotation);
                    revMon.GetComponent<MonsterStats>().ChangeDirection();
                    if (revMon.tag.Equals(curMon.name))
                    {
                        monsterSum += revMon.GetComponent<MonsterStats>().quantity;
                    }
                    Destroy(revMon, 15f);
                }

            }
            else if (Time.time > nextSpawnBiased) //SPAWN BIASED CREATURES FOR PROMPT
            {
                nextSpawnBiased = Time.time + spawnRateBiased;
                Transform ranTran2 = GetRandomSpawn();
                GameObject biasedMon = Instantiate(curMon.monster, ranTran2.position, ranTran2.rotation);
                monsterSum++;
                Destroy(biasedMon, 15f);
            }
        }
    }

    //Score algorithm, called per player
    int CalculateScore(int estimate, int actual)
    {
        //Rounds build in points
        //Max is 150pts / 1: 10 / 2: 20 / 3: 30 / 4: 40 / 5: 50 For Correct
        int points = 0;
        if (currentRound == 1) points = 10;
        else if (currentRound == 2) points = 20;
        else if (currentRound == 3) points = 30;
        else if (currentRound == 4) points = 40;
        else if (currentRound == 5) points = 50;

        //Off-estimate algorithm: max((actual-|actual-estimate|^2), 0)
        return (int)Mathf.Max(0, points - Mathf.Pow(Mathf.Abs(actual - estimate), 2));
    }
}
