using UnityEngine;

[System.Serializable]
public class SpinReward : ISpinReward
{
    public Currency rewardCurrency;
    public int rewardAmount;
    public int additionTurnAmount;

    public Sprite RewardIcon
    {
        get
        {
            return ResourceManager.Instance.GetCurrencySprite(rewardCurrency);
        }
    }

    public Currency? GetCurrency()
    {
        return rewardCurrency;
    }

    public bool IsBomb()
    {
        return false;
    }

    public int GetAmount(int turnsNotClaimed = 0)
    {
        return rewardAmount + (additionTurnAmount * turnsNotClaimed);
    }

    public Sprite GetIcon()
    {
        return RewardIcon;
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
