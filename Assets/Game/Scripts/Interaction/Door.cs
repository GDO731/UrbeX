using UnityEngine;

namespace Assets.Game.Scripts.Interaction
{
    public class Door : Interactable
    {
        [SerializeField] bool isOpen;
        [SerializeField] bool isLocked;

        static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            SetDoor(isOpen);
        }

        public override bool Interact()
        {
            if (isLocked) return false;
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
