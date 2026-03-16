using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class MainProgressIndicatorItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indicatorText;
    [SerializeField] private TextMeshProUGUI indicatorNextText;

    [SerializeField] private Image indicatorBackground;
    [SerializeField] private Image indicatorNextBackground;

    [SerializeField] private RectTransform mainProgressIndicatorItemContainer;

    public void Set(string text, string nextText, Sprite backgroundSprite, Sprite nextBackgroundSprite, Color textColor, Color nextTextColor)
    {
        indicatorText.text = text;
        indicatorNextText.text = nextText;
        indicatorBackground.sprite = backgroundSprite;
        indicatorNextBackground.sprite = nextBackgroundSprite;
        indicatorText.color = textColor;
        indicatorNextText.color = nextTextColor;
    }

    public Tween Animate()
    {
        return mainProgressIndicatorItemContainer.DOAnchorPosX(-82, 0.5f).OnComplete(() =>
        {
            mainProgressIndicatorItemContainer.anchoredPosition = Vector2.zero;
        });
    }
}
