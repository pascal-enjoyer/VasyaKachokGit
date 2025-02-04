using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]

public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public int baseDamage = 10;
    public Mesh weaponMesh;
    public Material baseMaterial;


    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip attackSound;


}

