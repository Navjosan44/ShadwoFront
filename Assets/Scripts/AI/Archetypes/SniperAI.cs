using UnityEngine;

namespace Shadowfront.AI.Archetypes
{
    public class SniperAI : EnemyAI
    {
        bool isAiming;
        float aimTime;
        
        protected override void Awake()
        {
            base.Awake();
            // Sniper is stationary - very slow movement
            if(agent != null && data != null)
            {
                agent.speed = data.moveSpeed * 0.4f;
                agent.updateRotation = false;
            }
        }
        
        protected override void DetectPlayer()
        {
            // Sniper has very long detection range
            if(player == null) return;
            float dist = Vector3.Distance(transform.position, player.position);
            float detectionRange = (data?.detectionRange ?? 20f) * 2.5f;
            bool canSee = dist < detectionRange && HasLineOfSight(player.position);
            
            if(canSee)
            {
                lastKnownPosition = player.position;
                float attackRange = data?.attackRange ?? 40f;
                if(dist <= attackRange) state = EnemyState.Attack;
                else if(state == EnemyState.Patrol || state == EnemyState.Alert) state = EnemyState.Chase;
            }
            else if(state == EnemyState.Chase && dist > detectionRange * 1.5f)
            {
                state = EnemyState.Alert;
            }
        }
        
        protected override void Attack()
        {
            if(player == null) return;
            
            // Sniper aims before shooting (telegraphed)
            float attackRange = data?.attackRange ?? 40f;
            float attackCooldown = data?.attackCooldown ?? 3f;
            int attackDamage = data?.attackDamage ?? 40;
            float accuracy = data?.accuracy ?? 0.8f;
            
            float dist = Vector3.Distance(transform.position, player.position);
            
            // Face player
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
            
            if(dist > attackRange)
            {
                state = EnemyState.Chase;
                isAiming = false;
                return;
            }
            
            // Aim phase (telegraph)
            if(!isAiming)
            {
                isAiming = true;
                aimTime = 0;
            }
            
            aimTime += Time.deltaTime;
            
            // After 1.5s aim, fire
            if(aimTime >= 1.5f && attackTimer <= 0)
            {
                if(Random.value <= accuracy) Shoot(attackDamage);
                attackTimer = attackCooldown;
                isAiming = false;
                aimTime = 0;
            }
        }
        
        protected override void UpdateAnimator()
        {
            base.UpdateAnimator();
            if(anim != null) anim.SetBool(\"IsAiming\", isAiming);
        }
        
        protected override bool FindCover()
        {
            // Sniper never seeks cover - it's a stationary long-range threat
            return false;
        }
        
        protected override void Shoot(int damage)
        {
            // Sniper: hitscan with tracer
            if(muzzleFlash != null) muzzleFlash.Play();
            var pc = player.GetComponent<Shadowfront.Player.PlayerController>();
            if(pc != null)
            {
                var h = pc.GetComponent<Health>();
                if(h != null) h.TakeDamage(damage);
            }
            if(fireSound != null) AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
    }
}
