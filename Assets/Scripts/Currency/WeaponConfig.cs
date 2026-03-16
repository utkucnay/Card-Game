using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct WeaponData
{
    public Weapon weapon;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Config/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private List<WeaponData> weapons;

    public Sprite GetWeaponIcon(Weapon weapon)
    {
        WeaponData data = weapons.Find(w => w.weapon == weapon);
        return data.icon;
    }
}