using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int maxHP;
    private int currentHP;
    [SerializeField] private int attack;
    [SerializeField] private int speed;
    [SerializeField] private int level;

    private void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// The attack system working is dependent on a D20 dice roll.
    /// A random number is generated. Based on the number, an attack is initiated.
    /// <para>1 - 3 => Miss</para>
    /// <para>4 - 10 => Weak</para>
    /// <para>11 - 17 => Normal</para>
    /// <para>18 - 20 => Crit</para>
    /// </summary>
    /// <returns>Attack</returns>
    public int Attack()
    {
        // roll a d20
        int random20 = Random.Range(1, 21);    // 1-20
        // Make the attack
        if (random20 <= 3)    // 1-3
        {
            return -1;
        }

        else if (random20 <= 10)    // 4-10
        {
            return attack - (attack * 20 / 100);    // attack - 20%attack
        }

        else if (random20 <= 17)    // 11-17
        {
            return attack;
        }

        else    // 18-20
        {
            return attack * 2;
        }
    }

    /// <summary>
    /// Deals damage to the unit and returns true if the unit is alive. Otherwise false.
    /// </summary>
    /// <param name="damage">Damage to be dealt</param>
    /// <returns></returns>
    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        return currentHP <= 0;
    }

    /// <summary>
    /// Resets the HP.
    /// This function can be used if a player flee from a battle in case we want to restore the enemies hp.
    /// <para>It can also be used when player interacts with a save point</para>
    /// </summary>
    public void ResetHP()
    {
        currentHP = maxHP;
    }

    // Getters
    public string GetName()
    {
        return unitName;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetSpeed()
    {
        return speed;
    }
}
