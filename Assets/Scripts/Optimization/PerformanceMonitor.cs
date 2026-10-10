using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.Optimization
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }
        
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size = 10;
            public bool expandable = true;
        }
        
        public Pool[] pools;
        Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();
        
void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(transform.root); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
            CreatePools();
        }
        
        void CreatePools()
        {
            foreach(var p in pools)
            {
                var queue = new Queue<GameObject>();
                for(int i=0;i<p.size;i++)
                {
                    var obj = Instantiate(p.prefab);
                    obj.name = p.tag + \"_\" + i;
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }
                poolDict[p.tag] = queue;
            }
        }
        
        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if(!poolDict.TryGetValue(tag, out var queue))
            {
                Debug.LogWarning($\"[ObjectPool] Pool '{tag}' not found\");
                return null;
            }
            
            GameObject obj = null;
            while(queue.Count > 0)
            {
                obj = queue.Dequeue();
                if(obj != null) break;
            }
            
            if(obj == null)
            {
                // Try to find an inactive one
                obj = new GameObject(tag + \"_Pool\");
            }
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        
        public void Despawn(string tag, GameObject obj)
        {
            if(obj == null) return;
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            if(poolDict.ContainsKey(tag))
                poolDict[tag].Enqueue(obj);
        }
        
        public int GetActiveCount(string tag)
        {
            if(poolDict.TryGetValue(tag, out var queue))
                return queue.Count;
            return 0;
        }
    }
    
    public class PerformanceMonitor : MonoBehaviour
    {
        public static PerformanceMonitor Instance { get; private set; }
        
        [Header(\"Budgets\")]
        public float frameTimeBudget = 16.67f; // 60 FPS
        public float memoryBudgetMB = 512f;
        public float loadTimeBudget = 3f;
        
        float[] frameTimes = new float[60];
        int frameIndex;
        float avgFrameTime;
        float peakFrameTime;
        float memoryUsage;
        
        public float AvgFrameTime => avgFrameTime;
        public float PeakFrameTime => peakFrameTime;
        public float FPS => 1000f / Mathf.Max(avgFrameTime, 0.001f);
        public float MemoryUsageMB => memoryUsage;
        public bool IsOverBudget => avgFrameTime > frameTimeBudget;
        
        void Awake()
        {
            if(Instance!=null && Instance!=this){ Destroy(GameObject); return; }
            Instance=this; DontDestroyOnLoad(transform.root);
        }
        
        void Update()
        {
            float frameTime = Time.unscaled_deltaTime * 1000f;
            frameTimes[frameIndex] = frameTime;
            frameIndex = (frameIndex + 1) % frameTimes.Length;
            
            // Calculate average
            float sum = 0;
            foreach(var ft in frameTimes) sum += ft;
            avgFrameTime = sum / frameTimes.Length;
            peakFrameTime = Mathf.Max(peakFrameTime, frameTime);
            
            memoryUsage = (float)System.GC.GetTotalMemory(false) / (1024f * 1024f);
        }
        
        public string GetPerformanceReport()
        {
            return $\"FPS: {FPS:F1} | AvgFrameTime: {avgFrameTime:F2}ms | Peak: {peakFrameTime:F2}ms | Memory: {memoryUsage:F1}MB | OverBudget: {IsOverBudget}\";
        }
        
        public void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
            frameTimeBudget = 1000f / fps;
        }
    }
}
