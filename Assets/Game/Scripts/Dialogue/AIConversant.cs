using Assets.Game.Scripts.Dialogue.Enums;
using Assets.Game.Scripts.Dialogue.Enums.Attributes;
using Assets.Game.Scripts.Interaction;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    public class AIConversant : Interactable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] Character conversant;
        [SerializeField] PlayerConversant playerConversant = null;

        public void TriggerDialogue()
        {
            Interact();
        }

        public override bool Interact()
        {
            base.Interact();

            if (dialogue == null) return false;

            playerConversant?.StartDialogue(this, dialogue);

            return true;
        }


        public string GetName()
        {
            return StringValueAttribute.GetStringValue(conversant);
        }

        public void UpdateDialogue(Dialogue dialogue)
        {
            this.dialogue = dialogue;
        }
    }
}
