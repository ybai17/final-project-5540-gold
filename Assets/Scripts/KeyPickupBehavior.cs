using UnityEngine;

/// <summary>
/// Defines which inventory slot this key belongs to.
/// </summary>
public enum KeyType
{
    Red   = 0,
    Green = 1,
    Blue  = 2,
}

public class KeyPickupBehavior : MonoBehaviour
{
    [Tooltip("Determines which hand‐slot this key will occupy.")]
    public KeyType keyType;

    [Tooltip("Reference to the key prefab to instantiate.")]
    public GameObject prefab;

    /// <summary>
    /// Called when the key has been picked up to remove it from the world.
    /// </summary>
    public void PickedUp()
    {
        Destroy(gameObject);
    }
}