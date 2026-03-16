using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class ProgressIndicator : MonoBehaviour
{
    [SerializeField] private ProgressIndicatorItem[] leftProgressIndicatorItem;
    [SerializeField] private ProgressIndicatorItem[] rightProgressIndicatorItem;
    [SerializeField] private RectTransform progressIndicatorContainer;

    [SerializeField] private MainProgressIndicatorItem mainProgressIndicatorItem;

    [SerializeField] private Sprite testSprite;

    private int maxProgress;
    private int currentProgress;

    public void Initialize(int currentProgress, int maxProgress)
    {
        this.currentProgress = currentProgress;
        this.maxProgress = maxProgress;
        leftProgressIndicatorItem = leftProgressIndicatorItem.Reverse().ToArray();
        UpdateProgress(currentProgress);
    }

    public void UpdateProgress(int currentProgress)
    {
        Color color1 = ResourceManager.Instance.GetZone(currentProgress).GetZoneColor();
        Color color2 = ResourceManager.Instance.GetZone(currentProgress + 1).GetZoneColor();
        Sprite sprite1 = ResourceManager.Instance.GetZone(currentProgress).GetZoneTopIndicatorIcon();
        Sprite sprite2 = ResourceManager.Instance.GetZone(currentProgress + 1).GetZoneTopIndicatorIcon();
        mainProgressIndicatorItem.Set(currentProgress.ToString(), (currentProgress + 1).ToString(), sprite1, sprite2, color1, color2);

        for (int i = 0; i < leftProgressIndicatorItem.Length; i++)
        {
            int progress = currentProgress - i;
            if (progress <= 0)
            {
                leftProgressIndicatorItem[i].Set(string.Empty, Color.white);
            }
            else
            {
                Color color = ResourceManager.Instance.GetZone(progress).GetZoneColor();
                leftProgressIndicatorItem[i].Set(progress.ToString(), color);
            }
        }

        for (int i = 0; i < rightProgressIndicatorItem.Length; i++)
        {
            int progress = currentProgress + i;

            if (progress > maxProgress)
            {
                rightProgressIndicatorItem[i].Set(string.Empty, Color.white);
            }
            else
            {
                Color color = ResourceManager.Instance.GetZone(progress).GetZoneColor();
                rightProgressIndicatorItem[i].Set(progress.ToString(), color);
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void TestAnimateInit()
    {
        Initialize(1, 90);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void TestAnimateNext()
    {
        AnimateNext();
    }
    
    public void AnimateNext()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(mainProgressIndicatorItem.Animate());
        sequence.Join(progressIndicatorContainer.DOAnchorPosX(-75, 0.5f));

        sequence.OnComplete(() =>
        {
            currentProgress++;
            UpdateProgress(currentProgress);
            progressIndicatorContainer.anchoredPosition = Vector2.zero;
        });
    }
}
