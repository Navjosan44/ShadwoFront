using UnityEngine;

namespace Shadowfront.AI.Archetypes
{
    public class HeavyAI : EnemyAI
    {
        int burstCount = 0;
        const int BURST_SIZE = 3;
        
        protected override void Awake()
        {
            base.Awake();
            // Heavy is slower but tankier - data should reflect this
            if(agent != null && data != null) agent.speed = data.moveSpeed * 0.7f;
        }
        
        protected override void Attack()
        {
            if(player == null) return;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            
            float attackRange = data?.attackRange ?? 4f;
            float attackCooldown = data?.attackCooldown ?? 1.5f;
            int attackDamage = data?.attackDamage ?? 15;
            
            // Burst fire
            if(attackTimer <= 0 && HasLineOfSight(player.position))
            {
                Shoot(attackDamage);
                burstCount++;
                if(burstCount >= BURST_SIZE)
                {
                    burstCount = 0;
                    attackTimer = attackCooldown;
                }
                else
                {
                    attackTimer = 0.15f; // rapid burst
                }
            }
            
            float dist = Vector3.Distance(transform.position, player.position);
            if(dist > attackRange * 1.5f) state = EnemyState.Chase;
        }
        
        protected override bool FindCover()
        {
            // Heavy doesn't seek cover often - it's a tank
            return Random.value < 0.1f && base.FindCover();
        }
    }
}
