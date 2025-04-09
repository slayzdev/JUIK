using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    private InventoryItem currentItem;
    
    public void Setup(InventoryItem item)
    {
        currentItem = item;
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        quantityText.text = item.isStackable ? item.quantity.ToString() : "";
    }
    
    public void OnItemClicked()
    {
        // Ajoutez ici la logique pour utiliser l'item
        Debug.Log($"Item cliqué : {currentItem.itemName}");
    }
} 