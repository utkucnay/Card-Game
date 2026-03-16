using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombReward : ISpinReward
{
    public bool IsBomb()
    {
        return true;
    }

    public int GetAmount(int turnsNotClaimed = 0)
    {
        return 0;
    }

    public Currency? GetCurrency()
    {
        return null;
    }

    public Sprite GetIcon()
    {
        return ResourceManager.Instance.GetBombSprite();
    }
    public Weapon? GetWeapon()
    {
        return null;
    }

    public GameCase? GetCase()
    {
        return null;
    }
}
