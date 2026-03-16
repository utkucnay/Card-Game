using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoneCondition
{
    bool IsSatisfied(int zoneIndex);
}
