using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class WeaponManager
{
    public static Dictionary<WeaponRarity, int> rarityProbabilities = new Dictionary<WeaponRarity, int>
        {
            { WeaponRarity.Common, 70 },    // 50% вероятность
            { WeaponRarity.Rare, 25 },      // 15% вероятность
            { WeaponRarity.Legendary, 5 }   // 1% вероятность
        };

    public static Color GetRarityColor(WeaponRarity rarity)
    {
        return rarity switch
        {
            WeaponRarity.Common => Color.gray,
            WeaponRarity.Rare => new Color(0.2f, 0.4f, 1f), // Синий
            WeaponRarity.Legendary => new Color(1f, 0.8f, 0f), // Золотой
            _ => Color.white
        };
    }
    public static int CalculateStats(WeaponData weaponData, WeaponRarity rarity)
    {
        return rarity switch
        {
            WeaponRarity.Common => weaponData.baseDamage,
            WeaponRarity.Rare => Mathf.RoundToInt(weaponData.baseDamage * 1.5f),
            WeaponRarity.Legendary => weaponData.baseDamage * 2,
            _ => weaponData.baseDamage
        };
    }

    public static WeaponRarity GetRandomWeaponRarity()
    {
        // Генерируем случайное число от 0 до 100
        System.Random random = new System.Random();
        double randomValue = random.NextDouble() * 100;

        // Проходим по всем редкостям и выбираем ту, которая соответствует случайному числу
        double cumulativeProbability = 0.0;
        foreach (var kvp in rarityProbabilities)
        {
            cumulativeProbability += kvp.Value;
            if (randomValue < cumulativeProbability)
            {
                return kvp.Key;
            }
        }

        // Если что-то пошло не так (например, сумма вероятностей не равна 100%), возвращаем Common
        return WeaponRarity.Common;
    }
}

