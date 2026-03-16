using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CurrencyData
{
    public Currency currency;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "CurrencyConfig", menuName = "Config/CurrencyConfig")]
public class CurrencyConfig : ScriptableObject
{
    [SerializeField] private List<CurrencyData> currencies;

    public Sprite GetCurrencyIcon(Currency currency)
    {
        CurrencyData data = currencies.Find(c => c.currency == currency);
        return data.icon;
    }
}
