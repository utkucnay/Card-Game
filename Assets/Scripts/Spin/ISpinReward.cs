using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpinReward
{
    bool IsBomb();
    Currency? GetCurrency();
    Weapon? GetWeapon();
    GameCase? GetCase();
    int GetAmount(int turnsNotClaimed = 0);
    Sprite GetIcon();
}
