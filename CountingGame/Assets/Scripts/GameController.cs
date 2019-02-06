using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Defined Monsters
    private List<Monster> definedMonsters;
    Monster curMon;

    //Textures
    public Texture picPurpleBall;
    public Texture picPurpleBird;

    //Spawn Points
    private List<Transform> spawnPoints;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;

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
    public GameObject playersPanel;
    public Text countText1;
    public Text countText2;

    //Reminder UI
    public GameObject reminderPanel;
    public RawImage remMonsterPic;

    //Other
    private bool canSpawn = false;
    private int monsterSum = 0;
    private int currentRound = 1;
    private GameLogic gameLogic;

    private void Start()
    {
        //Define Monsters
        Monster birdple = new Monster("birdple", "Wild and Fast.", picPurpleBird, purpleBird);
        Monster purble = new Monster("purble", "Careful, they bite!", picPurpleBall, purpleBallx1);

        //Make Lists
        monsters = new List<GameObject> { purpleBird, purpleBallx1, purpleBallx2, purpleBallx3, roborg };
        spawnPoints = new List<Transform> { spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4 };
        definedMonsters = new List<Monster> { birdple, purble };
        playerCountTexts = new List<Text> { countText1, countText2 };

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

    //Get player estimates from each player prefab in the game
    void GetPlayerEstimates()
    {
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i].GetComponent<Player>();
            playerCountTexts[i].text = p.GetPlayerCount().ToString();
            p.ResetPlayerCount();
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

        if (roundNumber < 4)
        {
            curMon = GetDefinedMonster();
            SetAllUI(roundNumber, curMon);
            StartCoroutine(StartRound());
        }
        else
        {
            print("Game Over");
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
        if (gameLogic != null) gameLogic.SetView("Ingame");
        startEndText.enabled = false;
        //reminderPanel.SetActive(true);
        animateUI.ShiftReminderPanel();
        canSpawn = true;
        yield return new WaitForSeconds(10f); // SPAWNING DISABLED AFTER X SECONDS
        canSpawn = false;
        yield return new WaitForSeconds(8f); // SHOW ROUND END TEXT / DISABLE COUNTING / HIDE REMINDER PANEL
        if (gameLogic != null) gameLogic.SetView("Start");
        startEndText.text = "FINISH!";
        startEndText.enabled = true;
        //reminderPanel.SetActive(false);
        animateUI.ShiftReminderPanel();
        GetPlayerEstimates();
        yield return new WaitForSeconds(2f); // SHOW RESULTS PANEL AND PLAYER ESTIMATES / HIDE ROUND END TEXT
        answerText.text = "THERE WERE...\n";
        //playersPanel.SetActive(true);
        animateUI.ShiftPlayersPanel();
        startEndText.enabled = false;
        resultsPanel.SetActive(true);
        yield return new WaitForSeconds(2f); // SHOW MONSTER SUM
        answerText.text = "THERE WERE...\n" + monsterSum;
        yield return new WaitForSeconds(2f); // AWARD POINTS TO PLAYERS
        yield return new WaitForSeconds(1f); // HIDE PLAYER PANEL AND RESULTS PANEL
        //playersPanel.SetActive(false);
        animateUI.ShiftPlayersPanel();
        resultsPanel.SetActive(false);
        yield return new WaitForSeconds(1f); // PREPARE FOR NEW ROUND
        SetRound(++currentRound);
    }

    //Update for instantiating monsters in a timely fashion
    void Update()
    {
        if (canSpawn)
        {
            if (Time.time > nextSpawnRandom)
            {
                nextSpawnRandom = Time.time + spawnRateRandom;
                Transform ranTran = GetRandomSpawn();
                GameObject mon = Instantiate(GetRandomMonster(), ranTran.position, ranTran.rotation);
                if (mon.tag.Equals(curMon.name))
                {
                    monsterSum += mon.GetComponent<MonsterStats>().quantity;
                }
                Destroy(mon, 15f);
            }
            else if (Time.time > nextSpawnBiased)
            {
                nextSpawnBiased = Time.time + spawnRateBiased;
                Transform ranTran2 = GetRandomSpawn();
                GameObject biasedMon = Instantiate(curMon.monster, ranTran2.position, ranTran2.rotation);
                monsterSum++;
                Destroy(biasedMon, 15f);
            }
        }


    }
}
