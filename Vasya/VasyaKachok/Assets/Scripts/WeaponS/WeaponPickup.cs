// WeaponPickup.cs
using UnityEngine;
using static WeaponData;

public class WeaponPickup : MonoBehaviour
{
    [Header("Settings")]
    public WeaponData weaponData;
    public WeaponRarity spawnRarity; // Можно сделать случайным
    public float rotationSpeed = 50f;

    [Header("References")]
    public Light pickupGlow;


    void Start()
    {
        InitializePickup();
    }

    void InitializePickup()
    {
        spawnRarity = WeaponManager.GetRandomWeaponRarity();
        pickupGlow.color = WeaponManager.GetRarityColor(spawnRarity);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PickUpWeapon(other.GetComponent<Inventory>());
        }
    }

    void PickUpWeapon(Inventory inventory)
    {
        if (inventory != null)
        {
            inventory.AddWeapon(weaponData, spawnRarity);
            Destroy(gameObject);
        }
    }


}
