using UnityEngine;
using ARMagicBar.Resources.Scripts.PlacementBar;

public class SinkTriggerZone : MonoBehaviour
{
    public GameObject cleanPlatePrefab;
    private InventoryAddable cleanPlateinventoryAddable;

    public AudioSource whooshSound;

    private InventoryAdder inventoryManager;

    private void Awake()
    {
        // Grab InventoryAdder from the scene at runtime
        inventoryManager = FindObjectOfType<InventoryAdder>();
        if (inventoryManager == null)
        {
            Debug.LogError("? InventoryAdder not found in scene.");
        }

        if (cleanPlatePrefab != null)
        {
            cleanPlateinventoryAddable = cleanPlatePrefab.GetComponent<InventoryAddable>();
        }

        if (cleanPlateinventoryAddable == null)
        {
            Debug.LogError("? Clean plate prefab is missing InventoryAddable component.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DirtyPlate"))
        {
            string plateName = other.gameObject.name;

            // Destroy dirty plate in scene
            Destroy(other.gameObject);

            // Play sound
            if (whooshSound != null)
                whooshSound.Play();

            // Add clean plate to inventory
            if (inventoryManager != null && cleanPlateinventoryAddable != null)
            {
                inventoryManager.AddToInventory(cleanPlateinventoryAddable.itemData);
                inventoryManager.RemoveItemByName("DirtyPlate");
                Debug.Log($"? Replaced dirty plate '{plateName}' with clean plate.");
            }
        }
    }
}
