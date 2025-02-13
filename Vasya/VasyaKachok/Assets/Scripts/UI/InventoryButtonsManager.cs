using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonsManager : MonoBehaviour
{

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


    }

    private void ToggleDropWeaponButton(bool isOn)
    {
        dropWeaponButton.gameObject.SetActive(isOn);
    }


}
