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

    private void Start()
    {
        currentHP = maxHP;
    }

    public int Attack()
    {
        // roll a d20
        int random20 = Random.Range(1, 21);    // 1-20
        // Make the attack
        if (random20 <= 3)    // 1-3
        {
            return 0;
        }

        else if (random20 <= 10)    // 4-10
        {
            return attack - (attack * 20 / 100);    // attack - 20%attack
        }

        else if (random20 <= 17)
        {
            return attack;
        }

        else
        {
            return attack * 2;
        }
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        return currentHP <= 0;
    }

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
}
