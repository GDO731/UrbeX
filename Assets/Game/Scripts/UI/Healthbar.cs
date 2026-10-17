using Assets.Project.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts.UI 
{
    public class Healthbar : MonoBehaviour
    {
        public Image fill;
        public Color fillColor = Color.green;

        private Health health;

        void Awake()
        {
            health = GetComponentInParent<Health>();
            health.OnHealthChanged += Refresh;
            fill.color = fillColor;
        }

        void OnDestroy()
        {
            if (health != null) health.OnHealthChanged -= Refresh;
        }

        void Refresh(int currentHealth, int maxHealth)
        {
            fill.fillAmount = Mathf.Max(0, (float)currentHealth / maxHealth);
        }
    }
}
