using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;
    
    private bool isInventoryOpen = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        inventoryPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }
    
    public void AddItem(InventoryItem newItem)
    {
        if (newItem.isStackable)
        {
            InventoryItem existingItem = items.Find(item => item.itemName == newItem.itemName);
            if (existingItem != null)
            {
                existingItem.quantity += newItem.quantity;
                return;
            }
        }
        
        items.Add(newItem);
    }
    
    public void RemoveItem(string itemName, int quantity = 1)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            if (item.isStackable)
            {
                item.quantity -= quantity;
                if (item.quantity <= 0)
                {
                    items.Remove(item);
                }
            }
            else
            {
                items.Remove(item);
            }
        }
    }
    
    private void UpdateInventoryUI()
    {
        // Nettoyer le conteneur
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Créer les éléments UI
        foreach (InventoryItem item in items)
        {
            GameObject itemUI = Instantiate(itemPrefab, itemContainer);
            InventoryItemUI itemUIComponent = itemUI.GetComponent<InventoryItemUI>();
            itemUIComponent.Setup(item);
        }
    }
} 