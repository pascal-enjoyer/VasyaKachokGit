using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{

    private List<WeaponPickup> availableToPickupWeapons = new List<WeaponPickup>();

    private WeaponPickup closestToPickupWeapon;

    public Button PickupButton;

    private void Start()
    {
        PickupButton.onClick.AddListener(OnPickupButtonClicked);
        PickupButton.gameObject.SetActive(closestToPickupWeapon != null);
    }

    private void Update()
    {
        if (availableToPickupWeapons.Count > 0)
        {
            if (availableToPickupWeapons.Count == 1)
            {
                closestToPickupWeapon = availableToPickupWeapons[0];
            }
            else
            {
                closestToPickupWeapon = FindClosestWeapon();
            }
        }
        
    }

    private WeaponPickup FindClosestWeapon()
    {
        float closestDistance = Mathf.Infinity;
        WeaponPickup newClosest = null;
        foreach (WeaponPickup weapon in availableToPickupWeapons)
        {
            if (weapon == null) continue;

            float distance = Vector3.Distance(
                transform.position,
                weapon.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                newClosest = weapon;
            }

        }

        return newClosest;
    }

    public void OnPickupButtonClicked()
    {
        if (gameObject.TryGetComponent<Inventory>(out Inventory inventory) && closestToPickupWeapon != null)
        {
            inventory.AddWeapon(closestToPickupWeapon.weaponData, closestToPickupWeapon.spawnRarity);
            OnPlayerLeaveFromPickupRange(closestToPickupWeapon);
            Destroy(closestToPickupWeapon.gameObject);
            closestToPickupWeapon = null;
        }
    }

    public void OnPlayerInPickupRange(WeaponPickup weaponPickup)
    {
        availableToPickupWeapons.Add(weaponPickup); 
        PickupButton.gameObject.SetActive(true);
    }

    public void OnPlayerLeaveFromPickupRange(WeaponPickup weaponPickup)
    {
        availableToPickupWeapons.Remove(weaponPickup);
        if (availableToPickupWeapons.Count == 0)
            PickupButton.gameObject.SetActive(false);
    }
 
}
