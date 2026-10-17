using Assets.Game.Scripts.Dialogue;
using Assets.Game.Scripts.Dialogue.Enums;
using Assets.Game.Scripts.Dialogue.Enums.Attributes;
using TMPro;
using UnityEngine;

namespace Assets.Game.Scripts.UI
{
    public class NameDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Name;
        void Start()
        {
            var aiConversant = GetComponent<AIConversant>();
            if (aiConversant != null)
            {
                Name.text = aiConversant.GetName();
            }
            else
            {
                Name.text = StringValueAttribute.GetStringValue(Character.Player);
            }
        }
    }
}
