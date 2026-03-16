using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RewardInfinityScrollVisualItem : InfiniteRectItem<RewardInfinityScrollItem>
{
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardText;

    public RectTransform RewardIconTransform => rewardIcon.transform as RectTransform;
    public RectTransform RectTransform => transform as RectTransform;
    public RewardInfinityScrollItem Data { get; private set; }

    public override void UpdateItem(RewardInfinityScrollItem data)
    {
        this.Data = data;
        rewardIcon.sprite = data.rewardSprite;

        rewardText.text = $"x{data.CurrentValue}";
    }

    private void Update()
    {
        if (Data != null)
        {
            rewardText.text = $"x{Data.CurrentValue}";
        }
    }
}
