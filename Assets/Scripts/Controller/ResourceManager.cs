using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    [SerializeField] private CurrencyConfig currencyConfig;
    [SerializeField] private WeaponConfig weaponConfig;
    [SerializeField] private CaseConfig caseConfig;
    [SerializeField] private ZoneConfig zoneConfig;
    [SerializeField] private Sprite bombSprite;

    public Sprite GetCurrencySprite(Currency currency)
    {
        return currencyConfig.GetCurrencyIcon(currency);
    }

    public Sprite GetBombSprite()
    {
        return bombSprite;
    }

    public Sprite GetWeaponIcon(Weapon weapon)
    {
        return weaponConfig.GetWeaponIcon(weapon);
    }

    public Sprite GetCaseIcon(GameCase caseReward)
    {
        return caseConfig.GetCaseIcon(caseReward);
    }

    public Zone GetZone(int zoneIndex)
    {
        return zoneConfig.GetZone(zoneIndex);
    }
}
