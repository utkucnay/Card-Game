[System.Serializable]
public class TrueZoneCondition : IZoneCondition
{
    public bool IsSatisfied(int zoneIndex)
    {
        return true;
    }
}
