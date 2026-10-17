using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Dialogue
{
    [System.Serializable]
    public class DialogueAction
    {
        public string action;
        public UnityEvent onTrigger;
    }
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] List<DialogueAction> dialogueActions = new List<DialogueAction>();

        public void Trigger(string actionToTrigger)
        {
            foreach (var dialogueAction in dialogueActions)
            {
                if (dialogueAction.action == actionToTrigger)
                {
                    dialogueAction.onTrigger.Invoke();
                }
            }
        }
    }
}
