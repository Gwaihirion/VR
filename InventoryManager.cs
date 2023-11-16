using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<InventoryItem> inventory = new List<InventoryItem>();

    // Add an item to the inventory
    public void AddItem(string name, GameObject prefab, int value, int quantity)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == name);
        if (existingItem != null)
        {
            // Item already exists, just update the quantity
            existingItem.quantity += quantity;
        }
        else
        {
            // Add new item to the inventory
            inventory.Add(new InventoryItem(name, prefab, value, quantity));
        }
    }

    // Remove an item or reduce its quantity
    public void RemoveItem(string name, int quantity)
    {
        InventoryItem item = inventory.Find(i => i.itemName == name);
        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                // If quantity falls to zero or below, remove the item from inventory
                inventory.Remove(item);
            }
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
        }
    }

    // Spawn an item object in the world
    public void SpawnItem(string name, Vector3 position)
    {
        InventoryItem item = inventory.Find(i => i.itemName == name);
        if (item != null && item.quantity > 0)
        {
            Instantiate(item.itemPrefab, position, Quaternion.identity);
            RemoveItem(name, 1); // Assuming we spawn one item at a time
        }
        else
        {
            Debug.LogWarning("Item not available or out of stock.");
        }
    }

    // Method to get the current inventory for display purposes
    public List<InventoryItem> GetInventory()
    {
        return inventory;
    }
}
