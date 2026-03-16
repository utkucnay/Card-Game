using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinRewardView : MonoBehaviour
{
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardText;

    public ISpinReward SpinReward { get; private set; }
    public ISpinReward SpinRewardOneTurn { get; private set; }

    public ISpinReward GetCurrentReward()
    {
        if (SpinRewardOneTurn != null)
        {
            return SpinRewardOneTurn;
        }
        return SpinReward;
    }

    public void SetReward(ISpinReward reward)
    {
        this.SpinReward = reward;
        rewardImage.sprite = reward.GetIcon();
        if (reward.IsBomb())
        {
            rewardText.text = "Bomb!";
        }
        else
        {
            rewardText.text = $"x{reward.GetAmount()}";
        }
    }

    public void SetRewardOneTurn(ISpinReward reward)
    {
        this.SpinRewardOneTurn = reward;
        rewardImage.sprite = reward.GetIcon();
        if (reward.IsBomb())
        {
            rewardText.text = "Bomb!";
        }
        else
        {
            rewardText.text = $"x{reward.GetAmount()}";
        }
    }

    public void CheckClearRewardOneTurn()
    {
        if (SpinRewardOneTurn != null)
        {
            SpinRewardOneTurn = null;
        }
        SetReward(SpinReward);
    }

    public void UpdateRotation(Quaternion parentRotation)
    {
        transform.localRotation = Quaternion.Inverse(parentRotation);
    }

    public void UpdateVisual(int turnsNotClaimed)
    {
        if (!SpinReward.IsBomb())
        {
            rewardText.text = $"x{SpinReward.GetAmount(turnsNotClaimed)}";
        }
    }
}
