using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpecialCaseSpinReward : ISpecialSpinReward
{
    [SerializeField] private GameCase caseReward;
    [SerializeField] private int amount;

    public bool IsBomb()
    {
        return false;
    }

    public Currency? GetCurrency()
    {
        return null;
    }

    public Weapon? GetWeapon()
    {
        return null;
    }

    public GameCase? GetCase()
    {
        return caseReward;
    }

    public int GetAmount(int turnsNotClaimed = 0)
    {
        return amount;
    }

    public Sprite GetIcon()
    {
        return ResourceManager.Instance.GetCaseIcon(caseReward);
    }
}
