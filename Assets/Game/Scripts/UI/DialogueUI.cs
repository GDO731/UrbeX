using Assets.Game.Scripts.Core;
using Assets.Game.Scripts.Dialogue;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI aiText;
        [SerializeField] TextMeshProUGUI conversantName;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;        
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject aiResponse;
        [SerializeField] GameObject choiceButtonPrefab;
        [SerializeField] PlayerConversant playerConversant;
        [SerializeField] AudioSource audioSource;

        void Start()
        {
            playerConversant.onConversationUpdated += UpdateDialogue;
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateDialogue();
        }

        private void UpdateDialogue()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) return;

            conversantName.text = playerConversant.GetCurrentConversantName();
            aiResponse.SetActive(!playerConversant.IsChosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChosing());

            quitButton.interactable = !playerConversant.HasNext();

            if (playerConversant.IsChosing())
            {
                BuildChoiceList();
            }
            else
            {
                var dialogueNode = playerConversant.GetNode();
                aiText.text = dialogueNode.GetText();

                var audioClip = dialogueNode.GetAudioClip();
                if(audioClip != null)
                {
                    SoundManager.instance.PlayClip(audioClip,audioSource);
                }

                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
            
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (var choice in playerConversant.GetChoices())
            {
                var choiceInstance = Instantiate(choiceButtonPrefab, choiceRoot);
                var textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = choice.GetText();


                var button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    var audioClip = choice.GetAudioClip();
                    if (audioClip != null)
                    {
                        DisableAllChoiceButtons();
                        SoundManager.instance.PlayClip(audioClip, audioSource);
                    }
                    
                    StartCoroutine(AdvanceAfterAudio());
                    playerConversant.SelectChoice(choice);
                });
            }
        }


        private void DisableAllChoiceButtons()
        {
            foreach (Transform item in choiceRoot)
            {
                var btn = item.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
        }

        IEnumerator AdvanceAfterAudio()
        {
            if(audioSource.isPlaying)
            {
                yield return null;
                yield return new WaitWhile(() => audioSource.isPlaying);
            }
            playerConversant.Next();
        }
    }
}
