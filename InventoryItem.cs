using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public GameObject itemPrefab;
    public int Value;
    public int quantity;

    public InventoryItem(string name, GameObject prefab, int Val, int qty)
    {
        itemName = name;
        itemPrefab = prefab;
        Value = Val;
        quantity = qty;
    }
}
