using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flight : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
