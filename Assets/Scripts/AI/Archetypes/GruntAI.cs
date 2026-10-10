using UnityEngine;

namespace Shadowfront.AI.Archetypes
{
    public class GruntAI : EnemyAI
    {
        protected override void Awake()
        {
            base.Awake();
            if(data != null && data.archetype != EnemyArchetype.Grunt)
                Debug.LogWarning(\"[GruntAI] Assigned non-Grunt EnemyData\");
        }
        
        protected override void Attack()
        {
            // Grunt: standard ranged attack, strafes slightly
            base.Attack();
        }
        
        protected override void Shoot(int damage)
        {
            // Grunt fires single shot
            base.Shoot(damage);
        }
    }
}
