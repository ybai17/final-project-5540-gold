using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages a fixed number of hand‐slots for the player.
/// Each slot holds exactly zero or one item; items cannot stack.
/// </summary>
public class PlayerInventoryManager : MonoBehaviour
{
    [Header("Inventory Slots (assign in Inspector)")]
    [Tooltip("Each Transform here is the pivot for one hand‐slot.")]
    public Transform[] handItemSlots;

    // Mirrors handItemSlots: null if empty, or holds the spawned GameObject if occupied.
    private GameObject[] itemsInSlots;

    /// <summary>
    /// Read‐only list of the items currently in hand, in slot order.
    /// </summary>
    public IReadOnlyList<GameObject> CurrentItemsInHand => itemsInSlots;

    void Awake()
    {
        // Initialize our tracking array to match the number of slots.
        itemsInSlots = new GameObject[handItemSlots.Length];


    }

    /// <summary>
    /// Picks up the given world‐space pickup and places it in its designated slot.
    /// </summary>
    public void AcquireItem(GameObject pickup)
    {
        if (pickup == null)
        {
            Debug.LogError("[Inventory] AcquireItem: pickup is null.");
            return;
        }

        // Get the pickup behavior to determine slot and prefab.
        var behavior = pickup.GetComponent<KeyPickupBehavior>();
        if (behavior == null || behavior.prefab == null)
        {
            Debug.LogError("[Inventory] AcquireItem: missing KeyPickupBehavior or prefab.");
            return;
        }

        int slotIndex = (int)behavior.keyType;

        // Validate that the slot index is within bounds.
        if (slotIndex < 0 || slotIndex >= handItemSlots.Length)
        {
            Debug.LogError($"[Inventory] Invalid slot index {slotIndex} for key type {behavior.keyType}.");
            return;
        }

        // Check if the slot is already occupied.
        if (itemsInSlots[slotIndex] != null)
        {
            Debug.LogWarning($"[Inventory] Slot {slotIndex} already occupied—cannot pick up another {behavior.keyType} key.");
            return;
        }

        // Instantiate the key prefab as a child of the designated slot pivot.
        GameObject clone = Instantiate(behavior.prefab, handItemSlots[slotIndex]);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;

        // Remove any Rigidbody so the key stays fixed in the hand.
        if (clone.TryGetComponent<Rigidbody>(out var rb))
            Destroy(rb);

        // Notify the pickup that it has been collected.
        behavior.PickedUp();

        // Show the slot pivot now that it has an item.
        handItemSlots[slotIndex].gameObject.SetActive(true);

        // Track the instantiated key in our array.
        itemsInSlots[slotIndex] = clone;
    }

    /// <summary>
    /// Uses (consumes) the specified item and frees up its slot.
    /// </summary>
    public void UseItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("[Inventory] UseItem: item is null.");
            return;
        }

        // Find which slot holds this instance.
        for (int i = 0; i < itemsInSlots.Length; i++)
        {
            if (itemsInSlots[i] == item)
            {
                // Destroy the item and clear the slot.
                Destroy(itemsInSlots[i]);
                itemsInSlots[i] = null;

                // Hide the now‐empty slot pivot.
                if (handItemSlots[i] != null)
                    handItemSlots[i].gameObject.SetActive(false);

                return;
            }
        }

        Debug.LogWarning("[Inventory] UseItem: item not found in any slot.");
    }
}