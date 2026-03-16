using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject for configuring spin rewards. Each reward has an ID, amount, and a flag indicating if it's a bomb (which could represent a negative outcome in the spin). This configuration can be used to define the rewards that players can receive when they spin a wheel or perform a similar action in the game.    
[CreateAssetMenu(fileName = "SpinRewardConfig", menuName = "Config/SpinRewardConfig")]
public class SpinRewardConfig : ScriptableObject
{
    [SerializeField] private List<SpinReward> rewards;

    public IReadOnlyList<SpinReward> Rewards => rewards;
}
