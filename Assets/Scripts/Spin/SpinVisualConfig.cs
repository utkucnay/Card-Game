using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject for storing visual configuration of Spin, such as colors, animation durations, etc.
[CreateAssetMenu(fileName = "SpinVisualConfig", menuName = "Config/SpinVisualConfig")]
public class SpinVisualConfig : ScriptableObject
{
    [Header("Colors")]
    public Color normalZoneColor;
    public Color safeZoneColor;
    public Color superZoneColor;

    public Sprite normalZoneSprite;
    public Sprite safeZoneSprite;
    public Sprite superZoneSprite;

    public Sprite normalZoneIndicatorSprite;
    public Sprite safeZoneIndicatorSprite;
    public Sprite superZoneIndicatorSprite;
}
