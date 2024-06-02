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
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    private Unit playerUnit;
    private Unit enemyUnit;
    private Unit[] turn;
    int turnIndex;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;

    private void Start()
    {
        BattleState = BattleState.START;

        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// SetupBattle is responsible for starting and setting up the battle.
    /// It takes references of player and enemy respectively.
    /// </summary>
    /// <returns>Waits for 2 seconds</returns>
    IEnumerator SetupBattle()
    {
        playerUnit = player.GetComponent<Unit>();
        enemyUnit = enemy.GetComponent<Unit>();

        // place the players and enemies in the scene
        dialogueText.text = "The " + enemyUnit.GetName() + " caught up to you!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

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
        turn = new Unit[2];
        turn[0] = playerUnit;
        turn[1] = enemyUnit;

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
            PlayerTurn();
        }

        else if (turn[turnIndex].tag == "Enemy")
        {
            BattleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        // Update the turn Index
        turnIndex++;
        if (turnIndex == turn.Length)
        {
            turnIndex = 0;
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
        int attack = playerUnit.Attack();
        bool isDead = false;

        if (attack == -1)
        {
            dialogueText.text = playerUnit.GetName() + " missed";
            attackType = AttackType.MISS;
        }
        else
        {
            if (attack < playerUnit.GetAttack())
            {
                dialogueText.text = "It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == playerUnit.GetAttack())
            {
                dialogueText.text = "It's a normal attack";
                attackType = AttackType.NORMAL;
            }
            else
            {
                dialogueText.text = "CRITICAL HIT!";
                attackType = AttackType.CRIT;
            }

            isDead = enemyUnit.TakeDamage(attack);
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
        }


        yield return new WaitForSeconds(1f);

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
        dialogueText.text = enemyUnit.GetName() + " attacks";

        yield return new WaitForSeconds(1f);

        int attack = enemyUnit.Attack();
        bool isDead = false;

        if (attack == -1)
        {
            dialogueText.text = enemyUnit.GetName() + " missed";
            attackType = AttackType.MISS;
        }
        else
        {
            if (attack < enemyUnit.GetAttack())
            {
                dialogueText.text = "It's a weak attack";
                attackType = AttackType.WEAK;
            }
            else if (attack == enemyUnit.GetAttack())
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
            isDead = playerUnit.TakeDamage(attack);
            playerHUD.SetHP(playerUnit.GetCurrentHP());
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
