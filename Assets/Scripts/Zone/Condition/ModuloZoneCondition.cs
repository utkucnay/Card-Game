using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModuloZoneCondition : IZoneCondition
{
    [SerializeField] public int divisor;

    public bool IsSatisfied(int zoneIndex)
    {
        return zoneIndex % divisor == 0;
    }
}