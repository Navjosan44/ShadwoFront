using UnityEngine;

namespace Shadowfront.AI.Archetypes
{
    public class ScoutAI : EnemyAI
    {
        protected override void Awake()
        {
            base.Awake();
            // Scout is faster
            if(agent != null && data != null) agent.speed = data.moveSpeed * 1.4f;
        }
        
        protected override void DetectPlayer()
        {
            // Scout has larger detection range
            if(player == null) return;
            float dist = Vector3.Distance(transform.position, player.position);
            float detectionRange = (data?.detectionRange ?? 20f) * 1.5f;
            bool canSee = dist < detectionRange && HasLineOfSight(player.position);
            
            if(canSee)
            {
                lastKnownPosition = player.position;
                float attackRange = data?.attackRange ?? 2.5f;
                if(dist <= attackRange) state = EnemyState.Attack;
                else if(state == EnemyState.Patrol || state == EnemyState.Alert) state = EnemyState.Chase;
            }
            else if(state == EnemyState.Chase && dist > detectionRange * 1.2f)
            {
                state = EnemyState.Alert;
            }
        }
        
        protected override void Chase()
        {
            // Scout flanks: approaches from side
            if(player == null) return;
            Vector3 flankDir = Vector3.Cross((player.position - transform.position).normalized, Vector3.up);
            Vector3 flankPos = player.position + flankDir * 3f;
            agent.SetDestination(flankPos);
            
            float dist = Vector3.Distance(transform.position, player.position);
            float attackRange = data?.attackRange ?? 2.5f;
            if(dist <= attackRange) state = EnemyState.Attack;
        }
        
        protected override void Attack()
        {
            // Scout: quick melee-style attack, low damage, fast cooldown
            if(player == null) return;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            float attackRange = data?.attackRange ?? 2.5f;
            float attackCooldown = data?.attackCooldown ?? 0.6f;
            int attackDamage = data?.attackDamage ?? 6;
            
            if(attackTimer <= 0 && HasLineOfSight(player.position))
            {
                Shoot(attackDamage);
                attackTimer = attackCooldown;
            }
            
            float dist = Vector3.Distance(transform.position, player.position);
            if(dist > attackRange * 1.5f) state = EnemyState.Chase;
        }
        
        protected override bool FindCover()
        {
            // Scout doesn't use cover - it's aggressive
            return false;
        }
    }
}
