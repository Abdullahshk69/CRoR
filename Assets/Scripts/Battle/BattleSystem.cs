using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// BattleStates define the state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    private BattleState state;

    [Header("GAME OBJECTS")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    private Unit playerUnit;
    private Unit enemyUnit;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;

    private void Start()
    {
        state = BattleState.START;

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnit = player.GetComponent<Unit>();
        enemyUnit = enemy.GetComponent<Unit>();

        // place the players and enemies in the scene
        dialogueText.text = "The " + enemyUnit.GetName() + " caught up to you!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {

        dialogueText.text = "Choose an action";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        // damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.Attack());

        enemyHUD.SetHP(enemyUnit.GetCurrentHP());
        dialogueText.text = "The attack is successful";

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Check if the enemy is dead
        // Change state based on what happened
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.GetName() + " attacks";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.Attack());

        playerHUD.SetHP(playerUnit.GetCurrentHP());

        if(isDead)
        {
            state = BattleState.LOST;
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You lost the battle!";
        }
    }
}
