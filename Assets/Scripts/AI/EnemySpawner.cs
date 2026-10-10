using UnityEngine;
using System.Collections.Generic;

namespace Shadowfront.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyData[] enemyTypes;
        public int maxEnemies = 10;
        public float spawnRadius = 30f;
        public float spawnInterval = 3f;
        public Transform[] spawnPoints;
        
        List<EnemyAI> activeEnemies = new List<EnemyAI>();
        float spawnTimer;
        bool spawning = true;
        
        public int ActiveEnemyCount => activeEnemies.Count;
        
        void Start()
        {
            if(spawnPoints == null || spawnPoints.Length == 0)
                GenerateSpawnPoints();
            spawnTimer = spawnInterval;
        }
        
        void GenerateSpawnPoints()
        {
            spawnPoints = new Transform[8];
            for(int i=0;i<8;i++)
            {
                var go = new GameObject($\"SpawnPoint_{i}\");
                float angle = (i / 8f) * Mathf.PI * 2;
                go.transform.position = transform.position + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
                spawnPoints[i] = go.transform;
            }
        }
        
        void Update()
        {
            if(!spawning) return;
            
            // Clean up dead enemies
            activeEnemies.RemoveAll(e => e == null || e.State == EnemyState.Dead);
            
            if(activeEnemies.Count < maxEnemies)
            {
                spawnTimer -= Time.deltaTime;
                if(spawnTimer <= 0)
                {
                    SpawnEnemy();
                    spawnTimer = spawnInterval;
                }
            }
        }
        
        public void SpawnEnemy(EnemyData specificType = null)
        {
            if(enemyTypes == null || enemyTypes.Length == 0) return;
            
            EnemyData type = specificType ?? enemyTypes[Random.Range(0, enemyTypes.Length)];
            if(type?.prefab == null) return;
            
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var enemy = Instantiate(type.prefab, spawnPoint.position, Quaternion.identity);
            var ai = enemy.GetComponent<EnemyAI>();
            if(ai != null)
            {
                ai.data = type;
                activeEnemies.Add(ai);
            }
        }
        
        public void StopSpawning() => spawning = false;
        public void StartSpawning() => spawning = true;
        
        public void SpawnWave(int count, EnemyData type = null)
        {
            for(int i=0;i<count;i++) SpawnEnemy(type);
        }
        
        public void ClearAll()
        {
            foreach(var e in activeEnemies)
                if(e != null) Destroy(e.gameObject);
            activeEnemies.Clear();
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
