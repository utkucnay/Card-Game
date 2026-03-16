using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressIndicatorItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indicatorText;

    public void Set(string text, Color textColor)
    {
        indicatorText.text = text;
        indicatorText.color = textColor;
    }
}
