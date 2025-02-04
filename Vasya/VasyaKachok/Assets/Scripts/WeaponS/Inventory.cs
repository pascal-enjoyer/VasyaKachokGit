// Inventory.cs
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;


public enum WeaponRarity { Common, Rare, Legendary }

public class Inventory : MonoBehaviour
{

    [System.Serializable]

    public class WeaponInstance
    {
        public WeaponData data;
        public WeaponRarity rarity;
    }

    public List<WeaponInstance> weapons = new List<WeaponInstance>();
    public Transform weaponParent;

    public void AddWeapon(WeaponData newData, WeaponRarity newRarity)
    {
        // ��������� ��� �� ��� ������ ������ ����� �� ��������
        if (!weapons.Exists(w => w.data == newData && w.rarity == newRarity))
        {
            weapons.Add(new WeaponInstance { data = newData, rarity = newRarity });
        }
        EquipWeapon(newData, newRarity);
    }

    public void EquipWeapon(WeaponData data, WeaponRarity rarity)
    {
        // ������� ������� ������
        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }

        // ������� ����� ���������
        GameObject newWeapon = Instantiate(data.weaponPrefab, weaponParent);
        EquipedWeapon weaponComponent = newWeapon.GetComponent<EquipedWeapon>();
        weaponComponent.Initialize(data, rarity);
    }

}
