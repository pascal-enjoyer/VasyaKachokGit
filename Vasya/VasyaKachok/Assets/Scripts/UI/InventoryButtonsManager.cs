using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonsManager : MonoBehaviour
{
    public PickupManager pickupManager;
    public Inventory inventory;

    public Button pickupButton;
    public Button dropWeaponButton;

    private void Start()
    {
        if (inventory != null)
        {

            inventory.WeaponEquiped.AddListener(ToggleDropWeaponButton);
            dropWeaponButton.onClick.AddListener(inventory.DropWeapon);
            ToggleDropWeaponButton(inventory.currentWeapon!=null);
        }

        if (pickupManager != null)
        {
            pickupButton.onClick.AddListener(pickupManager.OnPickupButtonClicked);
            pickupManager.WeaponInPickupZone.AddListener(TogglePickupButton);
            TogglePickupButton(pickupManager.closestToPickupWeapon != null);
        }
    }

    private void ToggleDropWeaponButton(bool isOn)
    {
        dropWeaponButton.gameObject.SetActive(isOn);
    }

    private void TogglePickupButton(bool isOn)
    {
        pickupButton.gameObject.SetActive(isOn);
    }
}
