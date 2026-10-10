using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.Data
{
    [System.Serializable]
    public class InventoryItem
    {
        public GameObject itemPrefab;
        public int count;
        public int maxStack = 99;
    }
    
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }
        public int capacity = 30;
        public List<InventoryItem> items = new List<InventoryItem>();
        
        public System.Action OnInventoryChanged;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(gameObject); return; }
            Instance=this; DontDestroyOnLoad(gameObject);
        }
        
        public bool AddItem(GameObject prefab, int count = 1)
        {
            if(prefab == null) return false;
            
            // Try stack
            var existing = items.Find(i => i.itemPrefab == prefab);
            if(existing != null)
            {
                int space = existing.maxStack - existing.count;
                int add = Mathf.Min(count, space);
                existing.count += add;
                count -= add;
                if(count == 0) { OnInventoryChanged?.Invoke(); return true; }
            }
            
            // New slot
            if(items.Count >= capacity) return false;
            
            items.Add(new InventoryItem { itemPrefab = prefab, count = count });
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public bool RemoveItem(GameObject prefab, int count = 1)
        {
            var item = items.Find(i => i.itemPrefab == prefab);
            if(item == null || item.count < count) return false;
            item.count -= count;
            if(item.count <= 0) items.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public int GetCount(GameObject prefab)
        {
            var item = items.Find(i => i.itemPrefab == prefab);
            return item?.count ?? 0;
        }
        
        public bool HasSpace => items.Count < capacity;
    }
}
