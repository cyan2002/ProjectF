using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyUIUpdate : MonoBehaviour
{
    public TextMeshProUGUI myTMPText;
    private int money;

    void Start()
    {
        myTMPText = GetComponent<TextMeshProUGUI>();
        this.money = MoneyManager.Instance.Money;
        MoneyManager.Instance.OnMoneyChanged += UpdateUI;
    }

    void UpdateUI(int newAmount){
        money = newAmount;
        myTMPText.text = "Money: " + money.ToString();
    }
}
