// WeaponPickup.cs
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Settings")]
    public WeaponData weaponData;
    public float rotationSpeed = 50f;
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("References")]
    public Light glowLight;

    private bool isInRange = false;
    private Inventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        InitializeVisuals();
    }

    void InitializeVisuals()
    {
        if (glowLight != null)
        {
            glowLight.color = weaponData.glowColor;
        }

        // Можно добавить дополнительные визуальные эффекты в зависимости от редкости
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(renderer.material);
            mat.SetColor("_EmissionColor", weaponData.glowColor);
            renderer.material = mat;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (isInRange && Input.GetKeyDown(pickupKey))
        {
            PickUpWeapon();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    void PickUpWeapon()
    {
        if (inventory != null)
        {
            inventory.AddWeapon(weaponData);

            Destroy(gameObject);
        }
    }

}