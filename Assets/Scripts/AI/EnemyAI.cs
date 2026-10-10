using UnityEngine;
using UnityEngine.AI;

namespace Shadowfront.AI
{
    public enum EnemyState { Patrol, Alert, Chase, Attack, Cover, Dead }
    
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class EnemyAI : MonoBehaviour
    {
        public EnemyData data;
        public Transform muzzle;
        public ParticleSystem muzzleFlash;
        public AudioClip fireSound;
        public Transform[] patrolPoints;
        
        protected NavMeshAgent agent;
        protected Health health;
        protected Animator anim;
        protected Transform player;
        protected EnemyState state = EnemyState.Patrol;
        protected int patrolIndex = 0;
        protected float patrolTimer;
        protected float attackTimer;
        protected Vector3 lastKnownPosition;
        protected Vector3 coverPosition;
        protected bool hasCover;
        
        public EnemyState State => state;
        
        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            anim = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag(\"Player\")?.transform;
            
            if(data != null)
            {
                health.max = data.maxHealth;
                agent.speed = data.moveSpeed;
            }
            
            if(patrolPoints == null || patrolPoints.Length == 0)
                GeneratePatrolPoints();
        }
        
        protected virtual void GeneratePatrolPoints()
        {
            float radius = data?.patrolRadius ?? 15f;
            patrolPoints = new Transform[4];
            for(int i=0;i<4;i++)
            {
                var go = new GameObject($\"PatrolPoint_{i}\");
                go.transform.position = transform.position + Random.insideUnitSphere * radius;
                go.transform.position = new Vector3(go.transform.position.x, transform.position.y, go.transform.position.z);
                patrolPoints[i] = go.transform;
            }
        }
        
        protected virtual void Update()
        {
            if(health.current <= 0)
            {
                if(state != EnemyState.Dead) Die();
                return;
            }
            
            DetectPlayer();
            attackTimer -= Time.deltaTime;
            
            switch(state)
            {
                case EnemyState.Patrol: Patrol(); break;
                case EnemyState.Alert: Alert(); break;
                case EnemyState.Chase: Chase(); break;
                case EnemyState.Attack: Attack(); break;
                case EnemyState.Cover: SeekCover(); break;
            }
            
            UpdateAnimator();
        }
        
        protected virtual void DetectPlayer()
        {
            if(player == null) return;
            float dist = Vector3.Distance(transform.position, player.position);
            float detectionRange = data?.detectionRange ?? 20f;
            bool canSee = dist < detectionRange && HasLineOfSight(player.position);
            
            if(canSee)
            {
                lastKnownPosition = player.position;
                float attackRange = data?.attackRange ?? 3f;
                if(dist <= attackRange) state = EnemyState.Attack;
                else if(state == EnemyState.Patrol || state == EnemyState.Alert) state = EnemyState.Chase;
            }
            else if(state == EnemyState.Chase && dist > detectionRange * 1.5f)
                state = EnemyState.Alert;
        }
        
        protected virtual bool HasLineOfSight(Vector3 target)
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 dir = (target - origin).normalized;
            float dist = Vector3.Distance(origin, target);
            return !Physics.Raycast(origin, dir, dist, ~LayerMask.GetMask(\"Player\"), QueryTriggerInteraction.Ignore);
        }
        
        protected virtual void Patrol()
        {
            if(patrolPoints == null || patrolPoints.Length == 0) return;
            if(agent.remainingDistance < 0.5f)
            {
                patrolTimer += Time.deltaTime;
                if(patrolTimer >= 2f)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[patrolIndex].position);
                    patrolTimer = 0;
                }
            }
        }
        
        protected virtual void Alert()
        {
            agent.SetDestination(lastKnownPosition);
            if(agent.remainingDistance < 1f) state = EnemyState.Patrol;
        }
        
        protected virtual void Chase()
        {
            if(player == null) return;
            agent.SetDestination(player.position);
            float dist = Vector3.Distance(transform.position, player.position);
            float attackRange = data?.attackRange ?? 3f;
            float detectionRange = data?.detectionRange ?? 20f;
            if(dist <= attackRange) state = EnemyState.Attack;
            else if(dist > detectionRange && !HasLineOfSight(player.position))
                state = EnemyState.Alert;
        }
        
        protected virtual void Attack()
        {
            if(player == null) return;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            float attackRange = data?.attackRange ?? 3f;
            float attackCooldown = data?.attackCooldown ?? 1f;
            int attackDamage = data?.attackDamage ?? 10;
            float accuracy = data?.accuracy ?? 0.6f;
            
            if(attackTimer <= 0 && HasLineOfSight(player.position))
            {
                if(Random.value <= accuracy) Shoot(attackDamage);
                attackTimer = attackCooldown;
            }
            
            float dist = Vector3.Distance(transform.position, player.position);
            if(dist > attackRange * 1.5f)
            {
                bool usesCover = data?.usesCover ?? true;
                if(usesCover && Random.value < 0.3f && FindCover()) state = EnemyState.Cover;
                else state = EnemyState.Chase;
            }
        }
        
        protected virtual void Shoot(int damage)
        {
            if(muzzleFlash != null) muzzleFlash.Play();
            var pc = player.GetComponent<Shadowfront.Player.PlayerController>();
            if(pc != null)
            {
                var h = pc.GetComponent<Health>();
                if(h != null) h.TakeDamage(damage);
            }
            if(fireSound != null) AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
        
        protected virtual bool FindCover()
        {
            Collider[] covers = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask(\"Default\"));
            float bestScore = -1;
            Vector3 bestPos = transform.position;
            
            foreach(var c in covers)
            {
                Vector3 toCover = (c.transform.position - transform.position).normalized;
                Vector3 toPlayer = (player.position - transform.position).normalized;
                float score = Vector3.Dot(-toPlayer, toCover);
                if(score > bestScore)
                {
                    bestScore = score;
                    bestPos = c.transform.position;
                }
            }
            
            if(bestScore > 0.3f)
            {
                coverPosition = bestPos;
                hasCover = true;
                return true;
            }
            return false;
        }
        
        protected virtual void SeekCover()
        {
            agent.SetDestination(coverPosition);
            if(agent.remainingDistance < 1f)
            {
                state = EnemyState.Attack;
                hasCover = false;
            }
        }
        
        protected virtual void UpdateAnimator()
        {
            if(anim == null) return;
            anim.SetFloat(\"Speed\", agent.velocity.magnitude / Mathf.Max(agent.speed, 0.1f));
            anim.SetBool(\"IsAttacking\", state == EnemyState.Attack);
            anim.SetBool(\"IsDead\", state == EnemyState.Dead);
        }
        
        protected virtual void Die()
        {
            state = EnemyState.Dead;
            agent.enabled = false;
            var col = GetComponent<Collider>();
            if(col != null) col.enabled = false;
            
            Events.GameEvents.EnemyKilled(transform.position);
            
            var mm = Missions.MissionManager.Instance;
            if(mm != null) mm.RegisterKill();
            
            var prog = Shadowfront.Data.Progression.Instance;
            if(prog != null && data != null)
            {
                prog.AddXP(data.xpReward);
                prog.RegisterKill();
            }
            
            if(data != null && data.lootTable != null && Random.value <= data.lootChance)
                data.lootTable.RollLoot(transform.position);
            
            Destroy(gameObject, 5f);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data?.detectionRange ?? 20f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data?.attackRange ?? 3f);
        }
    }
}
