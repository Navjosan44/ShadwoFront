using UnityEngine;

namespace Shadowfront.Core
{
    public class Health : MonoBehaviour
    {
        public int max = 100;
        public int current;
        
        public System.Action<int> OnDamaged;
        public System.Action OnDied;
        public System.Action OnHealed;
        
        void Awake(){ current = max; }
        
        public void TakeDamage(int d)
        {
            if(d <= 0) return;
            current -= d;
            OnDamaged?.Invoke(d);
            
            // Fire player events if this is the player
            if(gameObject.CompareTag(\"Player\"))
            {
                Events.GameEvents.PlayerDamaged(d);
                if(current <= 0) Events.GameEvents.PlayerDied();
            }
            
            if(current <= 0) Die();
        }
        
        public void Heal(int amount)
        {
            current = Mathf.Min(current + amount, max);
            OnHealed?.Invoke();
            if(gameObject.CompareTag(\"Player\")) Events.GameEvents.PlayerHealed();
        }
        
        void Die()
        {
            OnDied?.Invoke();
            gameObject.SetActive(false);
        }
        
        public float Fraction => (float)current / max;
    }
}
