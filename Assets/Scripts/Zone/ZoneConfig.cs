using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ZoneConfig", menuName = "Config/ZoneConfig")]
public class ZoneConfig : ScriptableObject
{
    [SerializeField] private Zone[] zones;

    public Zone GetZone(int zoneIndex)
    {
        foreach (Zone zone in zones.OrderByDescending(z => z.Priorty))
        {
            if (zone.IsActive(zoneIndex))
            {
                return zone;
            }
        }
        return null;
    }
}
