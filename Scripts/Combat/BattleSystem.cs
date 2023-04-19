using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    //All Fields (Ins Init = Inspector Initialized)
    //Temporary use
    [SerializeField] private GameObject[] players; //Ins init
    [SerializeField] private GameObject[] enemies; //Ins init
    //Battle object lists
    [Header ("Object lists")]
    [SerializeField] private List<GameObject> playerUnits = new List<GameObject>(); //These will be passed in
    [SerializeField] private List<GameObject> enemyUnits = new List<GameObject>(); //These will be generated
    [SerializeField] private List<Unit> playerScripts = new List<Unit>();
    [SerializeField] private List<Unit> enemyScripts = new List<Unit>();
    [SerializeField] private List<GameObject> playerStations = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyStations = new List<GameObject>();
    [SerializeField] private List<GameObject> turnOrder = new List<GameObject>();
    //Battle station prefabs
    [Header ("Battle station prefabs")]
    [SerializeField] private GameObject playerBattleStation; //Ins init
    [SerializeField] private GameObject enemyBattleStation; //Ins init
    //References for turn coroutines
    [Header ("Coroutine references")]
    [SerializeField] private GameObject activeUnit;
    [SerializeField] private GameObject targetUnit;
    [SerializeField] private Unit activeScript;
    [SerializeField] private Unit targetScript;
    [SerializeField] private bool waitForPlayer;
    [SerializeField] private bool targetingState;
    //Value dictating how quick health decreases when damage taken
    [SerializeField] private float healthFadeTime;
    //Values for organizing battle stations
    [Header ("Battle station organization values")]
    [SerializeField] private int maxRowLength;
    [SerializeField] private float stationSpacing;
    [SerializeField] private float stationZOffset;
    [SerializeField] private Vector3 center;
    //Values for turn order and current turn
    [Header ("Turn values")]
    [SerializeField] private int currentTurn; //Should start as 1
    [SerializeField] private int _orderIndex; //Should start as 0
    [Header ("State enum")] //State enum
    [SerializeField] private BattleState state;
    //References for UI components
    [Header ("Script references")]
    [SerializeField] private ECDetails detailsHUD; //Ins init
    [SerializeField] private Dialogue dialogueHUD; //Ins init
    [SerializeField] private Actions actionsHUD; //Ins init

    //All Properties
    public int OrderIndex
    {
        get => _orderIndex;
        private set
        {
            if (_orderIndex >= turnOrder.Count)
            {
                _orderIndex = 0;
                currentTurn++;
            }
            else
            {
                _orderIndex = value;
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < players.Length; i++)    //Manually populating the player list for now.
        {
            playerUnits.Add(players[i]);
            playerScripts.Add(players[i].GetComponent<Unit>());
        }
        
        for (int i = 0; i < enemies.Length; i++)    //Same for enemy list.
        {
            enemyUnits.Add(enemies[i]);
            enemyScripts.Add(enemies[i].GetComponent<Unit>());
        }
        state = BattleState.START;
        SetupBattle();
    }

    void Update()
    {
        if (targetingState == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 0;
            if (Physics.Raycast(ray, out hit, 500f, layerMask, QueryTriggerInteraction.Ignore))         //Perform constant raycast for mouse cursor while in targeting state.
            {
                if (hit.transform.gameObject.tag == "EnemyUnit")                                        //What raycast does while hovering over enemy.
                {
                    //enemyStations[enemyUnits.FirstOrDefault(x => x.Value == hit.transform.gameObject).Key].transform.position += new Vector3(0, 0.1f, 0);
                }
                if (hit.transform.gameObject.tag == "EnemyUnit" && Input.GetMouseButtonDown(0) == true) //What raycast does when clicking while hovering over enemy.
                {
                    targetUnit = hit.transform.gameObject;                                              //Set target unit for turn coroutine logic.
                    targetScript = targetUnit.GetComponent<Unit>();                                     //And their script.
                }
            }
        }
    }

    void SetupBattle() //All battle starting logic handled here.
    {
        SpawnEnemyUnits();
        SpawnAllStations();
        detailsHUD.SetDetails(playerScripts);
        dialogueHUD.SetText("The battle begins!");
        SetTurnOrder();
        StartCoroutine(Turn());
    }

    void SpawnAllStations()
    {
        float distanceP = (Math.Min(playerScripts.Count, maxRowLength) - 1) * stationSpacing;                               //Get total spread distance based on number of units. Clamped to max row length.
        for (int i = 0; i < playerScripts.Count; i++)
        {
            Vector3 position = new Vector3(((center.x - (distanceP / 2)) + ((i % maxRowLength) * stationSpacing)), center.y, (center.z - (stationZOffset * (1 + Mathf.Floor(i / maxRowLength))))); //Set position for next station.
            GameObject station = Instantiate(playerBattleStation, position, playerBattleStation.transform.rotation);        //Spawn station, then change name and add to dictionary.
            station.name = "PlayerSpot" + i.ToString();
            playerStations.Add(station);
            playerUnits[i].transform.position = new Vector3(position.x, playerUnits[i].transform.position.y, position.z);   //Move player character onto station.
        }
        float distanceE = (Math.Min(enemyScripts.Count, maxRowLength) - 1) * stationSpacing;                                //Get total spread distance based on number of units. Clamped to max row length.
        for (int i = 0; i < enemyScripts.Count; i++)
        {
            Vector3 position = new Vector3(((center.x - (distanceE / 2)) + ((i % maxRowLength) * stationSpacing)), center.y, (center.z + (stationZOffset * (1 + Mathf.Floor(i / maxRowLength))))); //Set position for next station.
            GameObject station = Instantiate(enemyBattleStation, position, enemyBattleStation.transform.rotation);          //Spawn station, then change name and add to dictionary.
            station.name = "EnemySpot" + i.ToString();
            enemyStations.Add(station);
            enemyUnits[i].transform.position = new Vector3(position.x, enemyUnits[i].transform.position.y, position.z);     //Move enemy character onto station.
        }
    }

    void SpawnEnemyUnits()
    {
        //List of enemies will be generated and spawned here.
    }

    void SetTurnOrder()
    {
        for (int i = 0; i < playerUnits.Count; i++)     //Populate turn order list from player roster.
        {
            turnOrder.Add(playerUnits[i]);
        }
        for (int i = 0; i < enemyUnits.Count; i++)      //Populate turn order list from enemy roster.
        {
            turnOrder.Add(enemyUnits[i]);
        }
        turnOrder.Sort(Unit.CompareUnitSpeed);          //Sort it by speed, then reverse it to be descending.
        turnOrder.Reverse();
    }

    IEnumerator Turn()
    {
        activeUnit = turnOrder[OrderIndex];                                                             //Get the unit whose turn it is.
        activeScript = activeUnit.GetComponent<Unit>();                                                 //And their script.

        state = activeScript.isPlayerUnit == true ? BattleState.PLAYERTURN : BattleState.ENEMYTURN;     //Determine whether it's player or enemy turn by checking active unit.

        yield return new WaitForSecondsRealtime(3f);                                                    //Wait for a second so the dialogue can be read.

        if (state == BattleState.PLAYERTURN)
        {
            PlayerTurn();
            yield return new WaitWhile(() => waitForPlayer == true);
        }
        yield return null;
    }

    IEnumerator PAttack()
    {
        //Set dialogue to reflect what's happening.
        dialogueHUD.SetText("Choose a target to attack.");
        //Enable cursor targeting of enemies and wait for player to select.
        targetingState = true;
        yield return new WaitWhile(() => targetUnit == null);
        //When selection made, disable targeting of enemies, disable cancel button interactability and dialogue displays to show the attack was made.
        targetingState = false;
        actionsHUD.SetCancelInteract(false);
        dialogueHUD.SetText(activeScript.unitName + " attacks " + targetScript.unitName + " for " + activeScript.damage.ToString() + " damage!");
        //Attack is made, re-enable the actions panel but disable interactability.
        bool isDead = targetScript.TakeDamage(activeScript.damage, healthFadeTime, detailsHUD.healthHandler);
        actionsHUD.SetActionsActive(true);
        actionsHUD.SetActionsInteract(false);
        //If the attacked unit has died, remove them from the turn order list.
        if (isDead == true)
        {
            turnOrder.Remove(targetUnit);
        }
        //End this by setting waitForPlayer to false and targetUnit/targetscript to null.
        waitForPlayer = false;
        targetUnit = null;
        targetScript = null;
        yield return null;
    }

    public void PlayerAttack()
    {
        StartCoroutine(PAttack());
    }

    public void PlayerTurn()
    {
        dialogueHUD.SetText("What will " + activeScript.unitName + " do?");
        actionsHUD.SetActionsInteract(true);
        if(targetingState == true)
        {
            targetingState = false;
        }
    }
}
