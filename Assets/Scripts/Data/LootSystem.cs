using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.Data
{
    [CreateAssetMenu(menuName=\"Shadowfront/Loot Table\")]
    public class LootTable : ScriptableObject
    {
        public LootEntry[] entries;
        
        [System.Serializable]
        public class LootEntry
        {
            public GameObject itemPrefab;
            public int minCount = 1;
            public int maxCount = 1;
            [Range(0f, 1f)] public float weight = 1f;
        }
        
        public List<GameObject> RollLoot(Vector3 position)
        {
            var results = new List<GameObject>();
            foreach(var e in entries)
            {
                if(Random.value <= e.weight)
                {
                    int count = Random.Range(e.minCount, e.maxCount + 1);
                    for(int i=0;i<count;i++)
                    {
                        var obj = Instantiate(e.itemPrefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
                        results.Add(obj);
                    }
                }
            }
            return results;
        }
    }
    
    public class LootItem : MonoBehaviour
    {
        public GameObject itemPrefab;
        public int count = 1;
        public float pickupRange = 1.5f;
        float bobOffset;
        
        void Update()
        {
            bobOffset += Time.deltaTime * 2f;
            transform.position += Vector3.up * Mathf.Sin(bobOffset) * 0.01f;
            transform.Rotate(0, 30f * Time.deltaTime, 0);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(\"Player\"))
            {
                var inv = Inventory.Instance;
                if(inv != null && inv.AddItem(itemPrefab, count))
                {
                    Events.GameEvents.LootCollected(gameObject);
                    var mm = Missions.MissionManager.Instance;
                    if(mm != null) mm.RegisterLoot();
                    var prog = Progression.Instance;
                    if(prog != null) prog.RegisterLoot();
                    Destroy(gameObject);
                }
            }
        }
    }
}
