using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecialWeaponSpinReward : ISpecialSpinReward
{
    [SerializeField] private Weapon weaponReward;
    [SerializeField] private int amount;

    public bool IsBomb()
    {
        return false;
    }

    public int GetAmount(int turnsNotClaimed = 0)
    {
        return amount;
    }

    public Currency? GetCurrency()
    {
        return null;
    }

    public Sprite GetIcon()
    {
        return ResourceManager.Instance.GetWeaponIcon(weaponReward );
    }

    public Weapon? GetWeapon()
    {
        return weaponReward;
    }

    public GameCase? GetCase()
    {
        return null;
    }
}
