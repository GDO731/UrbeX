using Assets.Game.Scripts.Dialogue;
using UnityEngine;

namespace Assets.Game.Scripts.Interaction
{
    public class Door : Interactable
    {
        [SerializeField] bool isOpen;
        [SerializeField] bool isLocked;
        [SerializeField] AIConversant lockedConversant;

        static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (lockedConversant == null) lockedConversant = GetComponentInChildren<AIConversant>();
        }

        void Start()
        {
            SetDoor(isOpen);
        }

        public override bool Interact()
        {

            if (isLocked)
            {
                lockedConversant?.TriggerDialogue();
                return false;
            }

            SetDoor(!isOpen);
            return true;
        }

        public void Unlock() => isLocked = false;

        void SetDoor(bool open)
        {
            isOpen = open;
            animator.SetBool(IsOpenHash, open);
        }
    }
}
