using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int quantity;
    public bool isStackable;
    public GameObject prefab;
} 