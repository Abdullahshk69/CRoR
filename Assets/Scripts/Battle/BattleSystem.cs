using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// BattleStates define the state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum AttackType { MISS, WEAK, NORMAL, CRIT, MAGIC }

public class BattleSystem : MonoBehaviour
{
    private BattleState BattleState;
    private AttackType attackType;

    [Header("GAME OBJECTS")]
    private GameObject[] players;
    private GameObject[] enemies;

    private Unit[] playerUnit;
    private Unit[] enemyUnit;
    private Unit[] turn;
    int turnIndex;
    int playerTurnCounter;
    int enemyTurnCounter;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] GameObject[] playerUI;
    [SerializeField] GameObject[] enemyUI;
    [SerializeField] GameObject UImenu;

    BattleHUD[] playerHUD;
    BattleHUD[] enemyHUD;

    private void Start()
    {
        BattleState = BattleState.START;

        players = GameObject.FindGameObjectsWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

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

    /// <summary>
    /// SetupBattle is responsible for starting and setting up the battle.
    /// It takes references of player and enemy respectively.
    /// </summary>
    /// <returns>Waits for 2 seconds</returns>
    IEnumerator SetupBattle()
    {

        dialogueText.text = "The battle has begun!";


        for (int i = 0; i < playerUnit.Length; i++)
        {
            playerHUD[i].SetHUD(playerUnit[i]);
        }

        for (int i = 0; i < enemyUnit.Length; i++)
        {
            enemyHUD[i].SetHUD(enemyUnit[i]);
        }

        foreach (GameObject hud in playerUI)
        {
            hud.SetActive(false);
        }

        yield return new WaitForSeconds(2f);

        SetTurns();
        TurnSelector();
    }

    /// <summary>
    /// SetTurns initializes and orders the turns array.
    /// It orders the array in descending order, by a Unit's speed attribute
    /// </summary>
    void SetTurns()
    {
        // populate the turn array
        turn = new Unit[playerUnit.Length + enemyUnit.Length];
        for (int i = 0; i < players.Length; i++)
        {

        }

        // Reorder the turns in descending order
        Array.Sort(turn,
            delegate (Unit x, Unit y) { return y.GetSpeed().CompareTo(x.GetSpeed()); }
            );

        // Set the turn index to 0
        turnIndex = 0;
    }

    /// <summary>
    /// TurnSelector selects the turn based on the tag of the game object.
    /// If it is a player's turn, then it calls the PlayerTurn function.
    /// Otherwise it starts the enemy turn coroutine.
    /// </summary>
    void TurnSelector()
    {
        // Check the tag of the object
        // Then call the turn function based on the tag
        if (turn[turnIndex].tag == "Player")
        {
            BattleState = BattleState.PLAYERTURN;
            playerTurnCounter++;
            playerUI[playerTurnCounter].gameObject.SetActive(true);
            UImenu.SetActive(true);
            PlayerTurn();
        }

        else if (turn[turnIndex].tag == "Enemy")
        {
            BattleState = BattleState.ENEMYTURN;
            enemyTurnCounter++;
            StartCoroutine(EnemyTurn());
        }

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
        if (BattleState != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    /// <summary>
    /// Player attacks an enemy.
    /// At the moment it is attacking only 1 enemy.
    /// In the future, we will provide the option to select the enemy to attack.
    /// </summary>
    /// <returns>Waits for 1 second</returns>
    IEnumerator PlayerAttack()
    {
        // damage the enemy
        int attack = playerUnit[playerTurnCounter].Attack();
        bool isDead = false;

        if (attack == -1)
        {
            dialogueText.text = playerUnit[playerTurnCounter].GetName() + " missed";
            attackType = AttackType.MISS;
        }
        else
        {
            if (attack < playerUnit[playerTurnCounter].GetAttack())
            {
                dialogueText.text = "It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == playerUnit[playerTurnCounter].GetAttack())
            {
                dialogueText.text = "It's a normal attack";
                attackType = AttackType.NORMAL;
            }
            else
            {
                dialogueText.text = "CRITICAL HIT!";
                attackType = AttackType.CRIT;
            }

            int rand = UnityEngine.Random.Range(0, enemyUnit.Length);
            isDead = enemyUnit[rand].TakeDamage(attack);
            enemyHUD[rand].SetHP(enemyUnit[rand].GetCurrentHP());
        }


        yield return new WaitForSeconds(1f);

        playerUI[playerTurnCounter].gameObject.SetActive(false);

        if (isDead)
        {
            BattleState = BattleState.WON;
            EndBattle();
        }
        else
        {
            BattleState = BattleState.ENEMYTURN;
            TurnSelector();
        }
        // Check if the enemy is dead
        // Change state based on what happened
    }

    /// <summary>
    /// Enemy attacks a player.
    /// Will randomize the player to be attacked in the Future
    /// </summary>
    /// <returns>Waits for 1 second</returns>
    IEnumerator EnemyTurn()
    {
        // Show all players
        // Hide the menu
        UImenu.SetActive(false);
        foreach (GameObject UI in playerUI)
        {
            UI.SetActive(true);
        }

        dialogueText.text = enemyUnit[enemyTurnCounter].GetName() + " attacks";

        yield return new WaitForSeconds(1f);

        int attack = enemyUnit[enemyTurnCounter].Attack();
        bool isDead = false;

        if (attack == -1)
        {
            dialogueText.text = enemyUnit[enemyTurnCounter].GetName() + " missed";
            attackType = AttackType.MISS;
        }
        else
        {
            if (attack < enemyUnit[enemyTurnCounter].GetAttack())
            {
                dialogueText.text = "It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == enemyUnit[enemyTurnCounter].GetAttack())
            {
                dialogueText.text = "It's a normal attack";
                attackType = AttackType.NORMAL;
            }
            else
            {
                dialogueText.text = "CRITICAL HIT!";
                attackType = AttackType.CRIT;
            }
            yield return new WaitForSeconds(1f);

            int rand = UnityEngine.Random.Range(0, players.Length);
            isDead = playerUnit[rand].TakeDamage(attack);
            playerHUD[rand].SetHP(playerUnit[rand].GetCurrentHP());
        }

        if (isDead)
        {
            BattleState = BattleState.LOST;
            EndBattle();
        }
        else
        {
            BattleState = BattleState.PLAYERTURN;
            TurnSelector();
        }
    }

    /// <summary>
    /// A generic function showing the outcome of a battle
    /// </summary>
    void EndBattle()
    {
        if (BattleState == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";

            // Add coroutine to show xp and stuff
        }
        else if (BattleState == BattleState.LOST)
        {
            dialogueText.text = "You lost the battle!";
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
}