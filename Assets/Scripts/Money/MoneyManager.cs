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

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        Money = 5000;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
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
