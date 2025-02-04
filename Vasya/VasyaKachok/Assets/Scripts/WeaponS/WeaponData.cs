using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip attackSound;



    public enum Rarity { Common, Rare, Legendary }

    [Header("Base Settings")]
    public string weaponName;
    public GameObject weaponPrefab;
    public int baseDamage = 10;

    [Header("Rarity Settings")]
    public Rarity rarity = Rarity.Common;
    public Color glowColor = Color.gray;

    public int CalculatedDamage
    {
        get
        {
            return rarity switch
            {
                Rarity.Common => baseDamage,
                Rarity.Rare => Mathf.RoundToInt(baseDamage * 1.5f),
                Rarity.Legendary => baseDamage * 2,
                _ => baseDamage
            };
        }
    }

    public Color GetRarityColor()
    {
        return rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Rare => new Color(0.5f, 0f, 1f), // Фиолетовый
            Rarity.Legendary => new Color(1f, 0.84f, 0f), // Золотой
            _ => Color.white
        };
    }
}
