
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Settings")]
    public WeaponData weaponData;
    public WeaponRarity spawnRarity; // Можно сделать случайным
    public float rotationSpeed = 50f;

    [Header("References")]
    public Light pickupGlow;

    public Transform posToSpawn;

    public void Start()
    {
        if (weaponData!=null)
            InitializePickup(weaponData, spawnRarity);

    }

    public void InitializePickup(WeaponData weaponData, WeaponRarity rarity)
    {
        this.weaponData = weaponData;
        Instantiate(weaponData.weaponPickupPrefab, posToSpawn);
        spawnRarity = rarity;
        pickupGlow.color = WeaponManager.GetRarityColor(spawnRarity);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }


    public void PickUpWeapon(Inventory inventory)
    {
        if (inventory != null)
        {
            inventory.AddWeapon(weaponData, spawnRarity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickupManager>(out PickupManager pickupManager))
        {
            pickupManager.OnPlayerInPickupRange(GetComponent<WeaponPickup>());
        }
    }

    

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PickupManager>(out PickupManager pickupManager))
        {
            pickupManager.OnPlayerLeaveFromPickupRange(GetComponent<WeaponPickup>());
        }

    }



}
