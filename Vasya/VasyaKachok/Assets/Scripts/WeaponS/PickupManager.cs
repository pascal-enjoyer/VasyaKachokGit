using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{

    private List<WeaponPickup> availableToPickupWeapons = new List<WeaponPickup>();

    public WeaponPickup closestToPickupWeapon;

    public UnityEvent<bool> WeaponInPickupZone;




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
            closestToPickupWeapon.PickUpWeapon(inventory);
            OnPlayerLeaveFromPickupRange(closestToPickupWeapon);
        }
    }

    public void OnPlayerInPickupRange(WeaponPickup weaponPickup)
    {
        WeaponInPickupZone?.Invoke(true);
        availableToPickupWeapons.Add(weaponPickup); 
    }

    public void OnPlayerLeaveFromPickupRange(WeaponPickup weaponPickup)
    {
        
        availableToPickupWeapons.Remove(weaponPickup);
        if (availableToPickupWeapons.Count == 0)
        {
            WeaponInPickupZone?.Invoke(false);
        }
    }
 
}
