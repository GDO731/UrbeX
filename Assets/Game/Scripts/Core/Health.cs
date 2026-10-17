using UnityEngine;

namespace Assets.Project.Scripts.Core

{
    public class Health : MonoBehaviour
    {
        public int maxHealth = 20;
        public int currentHealth = 20;

        public event System.Action<int, int> OnHealthChanged;
        public event System.Action<int> OnDamaged;
        public event System.Action OnDeath;

        public bool IsAlive => currentHealth > 0;

        public void Initialize(int maxHP)
        {
            maxHealth = maxHP;
            currentHealth = maxHP;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive) return;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnDamaged?.Invoke(amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            if (currentHealth <= 0) OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}