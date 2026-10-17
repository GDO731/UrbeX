using UnityEngine;

namespace Assets.Game.Scripts.MovementSounds
{
    public class FootstepManager : MonoBehaviour
    {
        // Audio Source and Clip Section
        [Header("Audio")]
        public AudioSource footstepSource;
        public AudioClip[] footstepClips; // Audio Clips were multiple can be added


        [Header("Settings")]
        public float stepInterval = 0.45f; // Interval Between Steps - Seconds
        public float rayDistance = 1.0f; // Raycast Distance below the player
        public LayerMask groundLayer;

        private float stepTimer;
        private CharacterController controller; // Looks for CharacterController - Needs to be child of the controller object

        void Start()
        {
            controller = GetComponentInParent<CharacterController>();

            if (footstepSource == null)
                footstepSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            // If there is no controller do not player sound
            if (controller == null) return;

            // If the controller is on the surface and velociy is high player the sound plus the time
            if (controller.isGrounded && controller.velocity.magnitude > 0.15f)
            {
                stepTimer += Time.deltaTime;

                if (stepTimer >= stepInterval)
                {
                    PlayFootstep();
                    stepTimer = 0f;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }

        void PlayFootstep()
        {
            // Raycast Script
            if (Physics.Raycast(transform.root.position + Vector3.up * 0.2f, Vector3.down, rayDistance, groundLayer))
            {
                if (footstepClips != null && footstepClips.Length > 0)
                {
                    AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
                    footstepSource.pitch = Random.Range(0.9f, 1.1f);
                    footstepSource.PlayOneShot(clip);
                }
            }
        }
    }
}