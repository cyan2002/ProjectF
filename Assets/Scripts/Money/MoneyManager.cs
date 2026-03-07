using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script deals with the money of the player.
/// Functions include:
/// -Money Total
/// -Checks if player has enough money to purchase
/// -Has an event when money changes (to change UI)
/// -Add Money
/// -Substract Money
/// </summary>
public class MoneyManager : MonoBehaviour
{
    //belongs to the class not the object, only one instance of this
    public static MoneyManager Instance { get; private set; }

    //this is a property not a field
    //Read-only from outside
    //Write-only inside the class
    //Prevents cheating / bugs
    public int Money { get; private set; }
    public int MoneyEarned { get; private set; }

    public event Action<int> OnMoneyChanged;

    public event Action addedMoney;

    private void Awake()
    {
        Money = 1000;
        MoneyEarned = 0;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        //temp goal
        MoneyEarned += amount;
        GameManager.Instance.UpdateGoalUI();

        OnMoneyChanged?.Invoke(Money);
        addedMoney?.Invoke();
    }

    public bool SpendMoney(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Money -= amount;
        OnMoneyChanged?.Invoke(Money);
        return true;
    }

    private bool CanAfford(int amount)
    {
        if(amount > Money)
        {
            return false;
        }
        else { return true; }
    }
}
