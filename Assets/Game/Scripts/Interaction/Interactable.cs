using UnityEngine;

namespace Assets.Game.Scripts.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] float radius = 3f;

        public float GetRange() => radius;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual bool Interact()
        {
            return true;
        }
    }
}