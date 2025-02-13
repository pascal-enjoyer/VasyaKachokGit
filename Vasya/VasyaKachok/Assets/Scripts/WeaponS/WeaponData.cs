using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]

public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPickupPrefab;
    public GameObject weaponInHandPrefab;
    public int baseDamage = 10;

    [Header("Для интерфейса")]
    public Sprite WeaponImageInUI;



}

