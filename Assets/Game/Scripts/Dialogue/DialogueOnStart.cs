using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    public class DialogueOnStart : MonoBehaviour
    {
        [SerializeField] private AIConversant conversant;
        [SerializeField] private float delay = 0.5f;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delay);
            conversant.Interact();
        }
    }
}
