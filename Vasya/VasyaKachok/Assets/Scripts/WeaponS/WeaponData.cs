using UnityEngine;


[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject, IInventoryVisuals
{
    public string weaponName;
    public GameObject weaponPickupPrefab;
    public GameObject weaponInHandPrefab;

    public int baseDamage = 10;
    public float damageByRarityMultiplier = 1.25f;

    public int attackSpeed = 1;
    public float attackSpeedByRarityMultiplier = 1f;

    [Header("Для интерфейса")]
    public Sprite WeaponImageInUI;
    public Sprite GetSprite()
    {
        return WeaponImageInUI;
    }




}

