using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<Currency, int> currency;
    private Dictionary<Weapon, int> weapons;
    private Dictionary<GameCase, int> cases;

    public Inventory()
    {
        currency = new Dictionary<Currency, int>();
        weapons = new Dictionary<Weapon, int>();
        cases = new Dictionary<GameCase, int>();
    }

    public void AddCurrency(Currency? currency, int amount)
    {
        if (currency == null)
        {
            return;
        }
        if (!this.currency.ContainsKey(currency.Value))
        {
            this.currency[currency.Value] = 0;
        }
        this.currency[currency.Value] += amount;
    }

    public int GetCurrencyAmount(Currency? currency)
    {
        if (currency == null)
        {
            return 0;
        }
        if (this.currency.TryGetValue(currency.Value, out int amount))
        {
            return amount;
        }
        return 0;
    }

    public void AddWeapon(Weapon? weapon, int amount)
    {
        if (weapon == null)
        {
            return;
        }
        if (!weapons.ContainsKey(weapon.Value))
        {
            weapons[weapon.Value] = 0;
        }
        weapons[weapon.Value] += amount;
    }

    public int GetWeaponAmount(Weapon? weapon)
    {
        if (weapon == null)
        {
            return 0;
        }
        if (weapons.TryGetValue(weapon.Value, out int amount))
        {
            return amount;
        }
        return 0;
    }

    public void AddCase(GameCase? gameCase, int amount)
    {
        if (gameCase == null)
        {
            return;
        }
        if (!cases.ContainsKey(gameCase.Value))
        {
            cases[gameCase.Value] = 0;
        }
        cases[gameCase.Value] += amount;
    }

    public int GetCaseAmount(GameCase? gameCase)
    {
        if (gameCase == null)
        {
            return 0;
        }
        if (cases.TryGetValue(gameCase.Value, out int amount))
        {
            return amount;
        }
        return 0;
    }
}
