using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ZoneSpecialType
{
    None,
    SpecialSpinRewardOverBomb,
    AllSpecialSpinReward
}

[CreateAssetMenu(fileName = "New Zone", menuName = "Config/Zone")]
public class Zone : ScriptableObject
{
    [Header("Condition")]
    [SerializeReference, SubclassSelector] private IZoneCondition zoneCondition;
    [SerializeField] private int priorty;
    [SerializeField] private bool isIncreaseRewardAmount;
    [SerializeField] private bool isLeave;

    [Header("Special")]
    [SerializeField] private ZoneSpecialType specialType;
    [SerializeReference, SubclassSelector] private ISpecialSpinReward[] specialSpinReward;

    [Header("Visual")]
    [SerializeField] private string zoneName;
    [SerializeField] private Sprite zoneIcon;
    [SerializeField] private Sprite zoneIndicatorIcon;
    [SerializeField] private Sprite zoneTopIndicatorIcon;
    [SerializeField] private Color zoneColor;

    public int Priorty => priorty;
    public bool IsLeave => isLeave;

    public ZoneSpecialType SpecialType => specialType;

    public bool IsActive(int zoneIndex)
    {
        return zoneCondition.IsSatisfied(zoneIndex);
    }

    public bool IsIncreaseRewardAmount()
    {
        return isIncreaseRewardAmount;
    }

    public Sprite GetZoneIcon()
    {
        return zoneIcon;
    }
    
    public Sprite GetZoneIndicatorIcon()
    {
        return zoneIndicatorIcon;
    }

    public Sprite GetZoneTopIndicatorIcon()
    {
        return zoneTopIndicatorIcon;
    }

    public string GetZoneName()
    {
        return zoneName;
    }

    public Color GetZoneColor()
    {
        return zoneColor;
    }

    public ISpecialSpinReward[] GetSpecialSpinRewards(int count)
    {
        return specialSpinReward.OrderBy(_ => RandomManager.Instance.GetRandomInt()).Take(count).ToArray();
    }
}
