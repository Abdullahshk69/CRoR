using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEngine.UI.CanvasScaler;

// BattleStates define the state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, FLEE, WON, LOST }
public enum AttackType { MISS, WEAK, NORMAL, CRIT, MAGIC }

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;

    private BattleState battleState;
    private AttackType attackType;

    [Header("GAME OBJECTS")]
    private GameObject player;
    private GameObject[] players;
    private GameObject[] enemies;
    [SerializeField] private GameObject prefabEnemy;
    [SerializeField] private GameObject prefabEnemyUI;
    [SerializeField] private GameObject parentEnemyDisplay;
    [SerializeField] private GameObject parentEnemyZone;

    private Unit[] playerUnit;
    private Unit[] enemyUnit;
    private Unit[] turn;
    int turnIndex;
    int playerTurnCounter;
    int enemyTurnCounter;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] GameObject[] playerUI;
    GameObject[] enemyUI;
    [SerializeField] GameObject UImenu;
    [SerializeField] GameObject optionsMenu;

    BattleHUD[] playerHUD;
    BattleHUD[] enemyHUD;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        battleState = BattleState.START;

        players = GameObject.FindGameObjectsWithTag("PlayerCombat");

        player = GameObject.FindGameObjectWithTag("Player");
        //FindObjects();
        RandomizeEnemy();
        DisablePlayerSpriteAndScript();

        SetEnemyScriptActive(false);

        //Debug.Log("Players size: " + players.Length);
        //Debug.Log("Enemies size: " + enemies.Length);

        playerTurnCounter = -1;
        enemyTurnCounter = -1;

        // place the players and enemies in the scene
        playerUnit = new Unit[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerUnit[i] = players[i].GetComponent<Unit>();
        }

        enemyUnit = new Unit[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyUnit[i] = enemies[i].GetComponent<Unit>();
        }

        // Add Huds to huds
        playerHUD = new BattleHUD[playerUI.Length];
        enemyHUD = new BattleHUD[enemyUI.Length];

        //Debug.Log("Players UI size: " + playerUI.Length);
        //Debug.Log("Enemies UI size: " + enemyUI.Length);

        for (int i = 0; i < playerHUD.Length; i++)
        {
            playerHUD[i] = playerUI[i].GetComponent<BattleHUD>();
        }

        for (int i = 0; i < enemyHUD.Length; i++)
        {
            enemyHUD[i] = enemyUI[i].GetComponent<BattleHUD>();
        }


        // Deactivate all Huds


        StartCoroutine(SetupBattle());
    }

    private void RandomizeEnemy()
    {
        int rand = UnityEngine.Random.Range(1, 5);
        enemies = new GameObject[rand];
        enemyUI = new GameObject[rand];

        // Instantiate Objects
        for (int i = 0; i < rand; i++)
        {
            enemies[i] = Instantiate(prefabEnemy);
            enemyUI[i] = Instantiate(prefabEnemyUI);
        }

        // Assign them parents
        for (int i = 0; i < rand; i++)
        {
            enemies[i].transform.SetParent(parentEnemyZone.transform, false);
            enemyUI[i].transform.SetParent(parentEnemyDisplay.transform, false);
            //enemies[i].transform.parent = parentEnemyZone.transform;
            //enemyUI[i].transform.parent = parentEnemyDisplay.transform;
            //parentEnemyZone.transform.parent = enemies[i].transform;
            //parentEnemyDisplay.transform.parent = enemyUI[i].transform;
        }
    }

    /// <summary>
    /// Find enemies gameObjects
    /// </summary>
    private void FindObjects()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    /// <summary>
    /// SetupBattle is responsible for starting and setting up the battle.
    /// It takes references of player and enemy respectively.
    /// </summary>
    /// <returns>Waits for 2 seconds</returns>
    IEnumerator SetupBattle()
    {

        dialogueText.text = "The battle has begun!";
        optionsMenu.SetActive(false);


        for (int i = 0; i < playerUnit.Length; i++)
        {
            playerHUD[i].SetHUD(playerUnit[i]);
        }

        for (int i = 0; i < enemyUnit.Length; i++)
        {
            enemyHUD[i].SetHUD(enemyUnit[i]);
        }

        DisablePlayerHUD();
        SetTurns();

        yield return new WaitForSeconds(2f);

        TurnSelector();
    }

    /// <summary>
    /// SetTurns initializes and orders the turns array.
    /// It orders the array in descending order, by a Unit's speed attribute.
    /// </summary>
    void SetTurns()
    {
        // populate the turn array
        turn = new Unit[playerUnit.Length + enemyUnit.Length];
        int counter = 0;
        foreach (Unit unit in playerUnit)
        {
            turn[counter++] = unit;
        }

        foreach (Unit unit in enemyUnit)
        {
            turn[counter++] = unit;
        }

        // Reorder the turns in descending order
        UnitSort(turn);
        UnitSort(playerUnit, playerHUD);
        UnitSort(enemyUnit, enemyHUD);

        // Set the turn index to 0
        turnIndex = 0;
    }

    /// <summary>
    /// Sort units
    /// </summary>
    /// <param name="units"></param>
    void UnitSort(Unit[] units)
    {
        Array.Sort(units, new UnitComparer());
    }

    /// <summary>
    /// Sort Hud based on units
    /// </summary>
    /// <param name="units"></param>
    /// <param name="huds"></param>
    void UnitSort(Unit[] units, BattleHUD[] huds)
    {
        Array.Sort(units, huds, new UnitComparer());
    }

    /// <summary>
    /// TurnSelector selects the turn based on the tag of the game object.
    /// If it is a player's turn, then it calls the PlayerTurn function.
    /// Otherwise it starts the enemy turn coroutine.
    /// </summary>
    void TurnSelector()
    {
        Debug.Log("TurnIndex == " + turnIndex);
        Debug.Log("Tag == " + turn[turnIndex].tag);
        // Check the tag of the object
        // Then call the turn function based on the tag
        DisablePlayerHUD();
        if (turn[turnIndex].tag == "PlayerCombat")
        {
            Debug.Log("Player's Turn at index == " + turnIndex);
            playerTurnCounter++;
            // check if the player is alive
            if (turn[turnIndex].IsDead())
            {
                Debug.Log("Dead player on index == " + turnIndex);
                NextTurn();
            }

            else
            {
                Debug.Log("Player is alive and can attack on turn == " + turnIndex);
                battleState = BattleState.PLAYERTURN;
                playerHUD[playerTurnCounter].gameObject.SetActive(true);
                UImenu.SetActive(true);
                optionsMenu.SetActive(true);
                PlayerTurn();
            }
        }

        else if (turn[turnIndex].tag == "Enemy")
        {
            enemyTurnCounter++;
            // Check if the enemy is alive
            if (turn[turnIndex].IsDead())
            {
                Debug.Log("Dead Enemy at index: " + turnIndex);
                NextTurn();
            }

            else
            {
                battleState = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
    }

    /// <summary>
    /// Updates the turn index.
    /// Resets the indeces if they go out of limit
    /// Then calls the TurnSelector()
    /// </summary>
    void NextTurn()
    {
        // Update the turn Index
        turnIndex++;
        if (turnIndex == turn.Length)
        {
            turnIndex = 0;
        }

        if (playerTurnCounter == players.Length - 1)
        {
            playerTurnCounter = -1;
        }

        if (enemyTurnCounter == enemies.Length - 1)
        {
            enemyTurnCounter = -1;
        }

        TurnSelector();
    }

    /// <summary>
    /// It only displays a text to let the player know it is their turn
    /// </summary>
    void PlayerTurn()
    {
        // Maybe enable buttons in the future when it is the player's turn
        dialogueText.text = "Choose an action";
    }

    /// <summary>
    /// If attack button is clicked and it is playerturn, then it starts the coroutine PlayerAttack
    /// </summary>
    public void OnAttackButton()
    {
        if (battleState != BattleState.PLAYERTURN)
        {
            return;
        }

        SetEnemyScriptActive(true);
        optionsMenu.SetActive(false);
        // Enable enemy Click Components
        //StartCoroutine(PlayerAttack());
    }

    /// <summary>
    /// Activate the ClickEnemy script
    /// </summary>
    /// <param name="active"></param>
    void SetEnemyScriptActive(bool active)
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<ClickEnemy>().enabled = active;
        }
    }

    /// <summary>
    /// A function to select an enemy. Using another script, an enemy can be selected.
    /// </summary>
    /// <param name="unit"></param>
    public void SelectEnemy(Unit unit)
    {
        if (unit.isActiveAndEnabled)
        {
            StartCoroutine(PlayerAttack(unit));
        }
    }

    /// <summary>
    /// Player attacks an enemy.
    /// At the moment it is attacking only 1 enemy.
    /// In the future, we will provide the option to select the enemy to attack.
    /// </summary>
    /// <returns>Waits for 1 second</returns>
    IEnumerator PlayerAttack(Unit unit)
    {
        SetEnemyScriptActive(false);
        // damage the enemy
        int attack = turn[turnIndex].Attack();
        bool isDead = false;
        int selectedEnemyIndex = 0;
        if (attack == -1)
        {
            dialogueText.text = turn[turnIndex].GetName() + " missed";
            attackType = AttackType.MISS;
            yield return new WaitForSeconds(1f);
            NextTurn();
        }
        else
        {
            if (attack < turn[turnIndex].GetAttack())
            {
                dialogueText.text = turn[turnIndex].GetName() + " It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == turn[turnIndex].GetAttack())
            {
                dialogueText.text = turn[turnIndex].GetName() + " It's a normal attack";
                attackType = AttackType.NORMAL;
            }
            else
            {
                dialogueText.text = turn[turnIndex].GetName() + " CRITICAL HIT!";
                attackType = AttackType.CRIT;
            }

            isDead = unit.TakeDamage(attack);
            for (int i = 0; i < enemyUnit.Length; i++)
            {
                if (enemyUnit[i] == unit)
                {
                    selectedEnemyIndex = i;
                    break;
                }
            }
            enemyHUD[selectedEnemyIndex].SetHP(enemyUnit[selectedEnemyIndex].GetCurrentHP());

            StartCoroutine(DamageFlash(enemyUnit[selectedEnemyIndex]));

            yield return new WaitForSeconds(1f);
            playerHUD[playerTurnCounter].gameObject.SetActive(false);

            if (isDead)
            {
                // Disable the deceased enemy and its UI
                DisableObject(enemyUnit[selectedEnemyIndex].gameObject);
                //enemyUnit[rand].gameObject.SetActive(false);

                DisableObject(enemyHUD[selectedEnemyIndex].gameObject);
                //enemyHUD[rand].gameObject.SetActive(false);

                // Check if all enemies are dead
                bool allEnemiesDead = true;
                foreach (Unit enemy in enemyUnit)
                {
                    if (!enemy.IsDead())
                    {
                        allEnemiesDead = false;
                        break;
                    }
                }
                if (allEnemiesDead)
                {
                    battleState = BattleState.WON;
                    EndBattle();
                }
                else
                {
                    Debug.Log("Next Turn from player at index" + turnIndex);
                    NextTurn();
                }
            }
            else
            {
                Debug.Log("Next Turn from player at index" + turnIndex);
                NextTurn();
            }
        }
    }

    /// <summary>
    /// Enemy attacks a player.
    /// Will randomize the player to be attacked in the Future
    /// </summary>
    /// <returns>Waits for 1 second</returns>
    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy: " + turn[turnIndex].name + "Fights! Index: " + turnIndex);
        // Show all alive players
        // Hide the menu
        UImenu.SetActive(false);
        EnablePlayerHUD();

        //dialogueText.text = turn[turnIndex].GetName() + " attacks";

        //yield return new WaitForSeconds(1f);

        int attack = turn[turnIndex].Attack();
        bool isDead = false;
        int rand = 0;

        if (attack == -1)
        {
            //dialogueText.text = turn[turnIndex].GetName() + " missed";
            attackType = AttackType.MISS;
            yield return new WaitForSeconds(1f);
            NextTurn();
        }
        else
        {
            if (attack < turn[turnIndex].GetAttack())
            {
                //dialogueText.text = "It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == turn[turnIndex].GetAttack())
            {
                //dialogueText.text = "It's a normal attack";
                attackType = AttackType.NORMAL;
            }
            else
            {
                //dialogueText.text = "CRITICAL HIT!";
                attackType = AttackType.CRIT;
            }

            do
            {
                rand = UnityEngine.Random.Range(0, players.Length);
                Debug.Log("Random: " + rand);
                Debug.Log("IsPlayerDead: " + playerUnit[rand].IsDead());
            } while (playerUnit[rand].IsDead());

            Debug.Log("Dealing Damage to player on index: " + rand + "\nPlayer name is " + playerUnit[rand].GetName());
            isDead = playerUnit[rand].TakeDamage(attack);
            playerHUD[rand].SetHP(playerUnit[rand].GetCurrentHP());

            StartCoroutine(DamageFlash(playerHUD[rand]));

            yield return new WaitForSeconds(2f);

            // Check if king is dead
            if (isDead && playerUnit[rand].GetName() == "King")
            {
                Debug.Log("Game Over");
                battleState = BattleState.LOST;
                EndBattle();
            }
            else
            {
                Debug.Log("Next Turn from enemy at index" + turnIndex);
                NextTurn();
            }
        }
    }

    /// <summary>
    /// A generic function showing the outcome of a battle
    /// </summary>
    void EndBattle()
    {
        UImenu.SetActive(true);
        optionsMenu.SetActive(false);
        if (battleState == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
            Invoke(nameof(EnablePlayerSpriteAndScript), 0.9f);

            //Win screen
            ReturnToMap();
        }
        else if (battleState == BattleState.LOST)
        {
            dialogueText.text = "You lost the battle!";
            // Lose Screen
            SceneController.instance.Lose();
        }
        else if (battleState == BattleState.FLEE)
        {
            dialogueText.text = "You successfully fled the battle!";
            Invoke(nameof(EnablePlayerSpriteAndScript), 0.9f);
            ReturnToMap();
        }
    }

    /// <summary>
    /// Gets the type of attack { MISS, WEAK, NORMAL, CRIT, MAGIC }
    /// </summary>
    /// <returns>Attack Type</returns>
    public AttackType GetAttackType()
    {
        return attackType;
    }

    public void OnFleeButton()
    {
        if (battleState != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(Flee());
    }

    /// <summary>
    /// Flee Mechanic:
    /// 50% chance to flee.
    /// You sacrifice your turn to flee.
    /// Turn ends regardless.
    /// </summary>
    /// <returns></returns>
    IEnumerator Flee()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand >= 50)
        {
            battleState = BattleState.FLEE;
            dialogueText.text = "Flee Successful!";
            yield return new WaitForSeconds(1f);
            EndBattle();
        }

        else
        {
            dialogueText.text = "Flee Unsuccessful!";
            yield return new WaitForSeconds(1f);
            NextTurn();
        }
    }

    /// <summary>
    /// Disable Player HUD
    /// </summary>
    void DisablePlayerHUD()
    {
        foreach (GameObject hud in playerUI)
        {
            hud.SetActive(false);
        }
    }

    /// <summary>
    /// Enable Player HUD
    /// </summary>
    void EnablePlayerHUD()
    {
        for (int i = 0; i < playerUnit.Length; i++)
        {
            if (!playerUnit[i].IsDead())
            {
                playerHUD[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// A general function to disable a game object
    /// </summary>
    /// <param name="gameObject"></param>
    void DisableObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Makes the enemy unit flash.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    IEnumerator DamageFlash(Unit unit)
    {
        VFXController.instance.moveVFX(unit.gameObject.transform);
        VFXController.instance.playVFX(2);
        yield return new WaitForSeconds(0.4f);

        unit.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        unit.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Makes the Player HUD flash.
    /// </summary>
    /// <param name="hud"></param>
    /// <returns></returns>
    IEnumerator DamageFlash(BattleHUD hud)
    {
        Transform portrait = hud.gameObject.transform.GetChild(0);
        Transform character = portrait.GetChild(1);

        //VFXController.instance.moveVFX(character.transform);
        //VFXController.instance.playVFX(3);
        //yield return new WaitForSeconds(0.4f);

        character.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        character.GetComponent<Image>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        character.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        character.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// Enables a player's world sprite and movement script.
    /// </summary>
    private void EnablePlayerSpriteAndScript()
    {
        player.GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Disables a player's world sprite and movement script.
    /// </summary>
    private void DisablePlayerSpriteAndScript()
    {
        player.GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Returns the player to the map where the battle got triggered from.
    /// </summary>
    private void ReturnToMap()
    {
        if (SceneController.instance.getScene() == "TopFloor - Hallway")
        {
            SceneController.instance.updateScene();
            SceneController.instance.ToHallway();
        }
        else if (SceneController.instance.getScene() == "TopFloor - Stairs")
        {
            SceneController.instance.updateScene();
            SceneController.instance.ToStairs();
        }
        else if (SceneController.instance.getScene() == "TopFloor -Throne Room")
        {
            SceneController.instance.updateScene();
            SceneController.instance.ToThroneRoom();
        }
    }
}

