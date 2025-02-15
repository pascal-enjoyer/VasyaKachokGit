using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PickupWeaponUIManager : MonoBehaviour
{
    public Button pickupButton;
    public WeaponData currentClosestWeapon;
    public Image weaponImageSlot;
    public PickupManager pickupManager;

    private void Start()
    {
        pickupManager.ClosestToPickupWeaponChanged.AddListener(OnClosestToPickupWeaponChanged);
        pickupManager.WeaponInPickupZone.AddListener(OnWeaponInPickupZone);

        pickupButton.onClick.AddListener(pickupManager.OnPickupButtonClicked);

        OnWeaponInPickupZone(pickupManager.closestToPickupWeapon != null);
    }
    public void OnWeaponInPickupZone(bool isInZone)
    {
        weaponImageSlot.enabled = isInZone;
        TogglePickupButton(isInZone);
    }

    public void OnClosestToPickupWeaponChanged(WeaponData newClosestWeapon)
    {
        weaponImageSlot.enabled=true;
        currentClosestWeapon = newClosestWeapon;
        weaponImageSlot.sprite = newClosestWeapon.WeaponImageInUI;
        
    }

    private void TogglePickupButton(bool isOn)
    {
        pickupButton.gameObject.SetActive(isOn);
    }
}
