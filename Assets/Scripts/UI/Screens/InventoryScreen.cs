using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Shadowfront.UI.Screens
{
    public class InventoryScreen : MonoBehaviour
    {
        public GameObject panel;
        public Transform itemsContainer;
        public GameObject itemSlotPrefab;
        public TextMeshProUGUI capacityText;
        public TextMeshProUGUI emptyText;
        public Button closeButton;
        
        Shadowfront.Data.Inventory inventory;
        
        void Awake()
        {
            panel?.SetActive(false);
            closeButton?.onClick.AddListener(Close);
            inventory = Shadowfront.Data.Inventory.Instance;
            if(inventory != null)
                inventory.OnInventoryChanged += Refresh;
        }
        
        void OnDestroy()
        {
            if(inventory != null)
                inventory.OnInventoryChanged -= Refresh;
        }
        
        public void Open()
        {
            panel?.SetActive(true);
            Refresh();
            Time.timeScale = 0f;
        }
        
        public void Close()
        {
            panel?.SetActive(false);
            Time.timeScale = 1f;
        }
        
        public void Refresh()
        {
            if(inventory == null) return;
            
            if(itemsContainer != null)
            {
                foreach(Transform child in itemsContainer) Destroy(child.gameObject);
                
                if(inventory.items.Count == 0 && emptyText != null)
                    emptyText.gameObject.SetActive(true);
                else if(emptyText != null)
                    emptyText.gameObject.SetActive(false);
                
                foreach(var item in inventory.items)
                {
                    var slot = Instantiate(itemSlotPrefab, itemsContainer);
                    var txt = slot.GetComponentInChildren<TextMeshProUGUI>();
                    if(txt != null && item.itemPrefab != null)
                        txt.text = $"{item.itemPrefab.name} x{item.count}";
                }
            }
            
            if(capacityText != null)
                capacityText.text = $"{inventory.items.Count}/{inventory.capacity}";
        }
        
        void Update()
        {
            if(panel != null && panel.activeSelf && Shadowfront.Input.InputManager.Instance?.PausePressed == true)
                Close();
        }
    }
}
