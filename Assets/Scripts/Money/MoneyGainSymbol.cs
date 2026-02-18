using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGainSymbol : MonoBehaviour
{
    public GameObject moneySymbolPrefab;
    public Transform symbolSpawnPoint;

    private void Start()
    {
        MoneyManager.Instance.addedMoney += ShowSymbol;
    }

    void ShowSymbol()
    {
        Instantiate(
            moneySymbolPrefab,
            symbolSpawnPoint.position,
            Quaternion.identity,
            symbolSpawnPoint
        );
    }
}
