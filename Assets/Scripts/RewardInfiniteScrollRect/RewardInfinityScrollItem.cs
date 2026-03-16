using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RewardInfinityScrollItem
{
    public string rewardCurrency;
    public Sprite rewardSprite;
    private int currentValue;
    private int targetValue;
    private int currentTweenId = 102304;

    public int CurrentValue => currentValue;

    public int TargetValue
    {
        get => targetValue;
        set
        {
            targetValue = value;

            DOTween.Kill(currentTweenId, true);
    
            DOVirtual.Int(currentValue, targetValue, 1f, value =>
            {
                currentValue = value;
            }).SetEase(Ease.OutCubic).SetDelay(0.1f).SetId(currentTweenId).OnComplete(() =>
            {
                currentValue = targetValue;
            });
        }
    }
}
