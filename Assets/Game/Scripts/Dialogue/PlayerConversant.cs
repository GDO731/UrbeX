using Assets.Game.Scripts.Core;
using Assets.Game.Scripts.Dialogue.Enums;
using Assets.Game.Scripts.Dialogue.Enums.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Scripts.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {

        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        [Header("Audio")]
        [SerializeField] AudioClip startClip;
        [SerializeField] AudioClip choiceClip;
        [SerializeField] AudioClip nextClip;
        [SerializeField] AudioClip quitClip;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            PlaySound(startClip);
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            PlaySound(quitClip);
            currentDialogue = null;
            TriggerExitAction();
            currentConversant = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentNode != null;
        }

        public bool IsChosing()
        {
            return isChoosing;
        }

        public DialogueNode GetNode()
        {
            if(currentNode == null) return null;

            return currentNode;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            PlaySound(choiceClip);
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            onConversationUpdated();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 1) 
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            if(HasNext())
            {
                PlaySound(nextClip);
                var childrenNodes = currentDialogue.GetAllChildren(currentNode);
                var index = UnityEngine.Random.Range(0, childrenNodes.Count());
                var nextNode = childrenNodes.ToList()[index];
                TriggerExitAction();
                if (nextNode.GetDelay() > 0)
                {
                    StartCoroutine(AdvanceAfterDelay(nextNode));
                }
                else
                {
                    AdvanceToNextDialogue(nextNode);
                }
            }
        }

        private void AdvanceToNextDialogue(DialogueNode nextNode)
        {
            currentNode = nextNode;
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return StringValueAttribute.GetStringValue(Character.Player);
            }
            else
            {
                return StringValueAttribute.GetStringValue(currentNode.GetCharacter());
            }
        }

        private void PlaySound(AudioClip clip)
        {
            SoundManager.instance.PlayClip(clip);
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null) 
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (string.IsNullOrEmpty(action)) return;

            foreach(var triggers in currentConversant.GetComponents<DialogueTrigger>())
            {
                triggers.Trigger(action);
            }
        }

        private IEnumerator AdvanceAfterDelay(DialogueNode nextNode)
        {
            yield return new WaitForSeconds(nextNode.GetDelay());
            AdvanceToNextDialogue(nextNode);
        }
    }
}
