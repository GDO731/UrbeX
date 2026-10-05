using UnityEngine;

namespace Assets.Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] float walkSpeed = 3.5f;
        [SerializeField] float runSpeed = 6f;
        [SerializeField] float gravity = -25f;
        [SerializeField] SpriteRenderer sprite;


        CharacterController controller;
        Transform cam;
        InputSystem_Actions controls;
        float verticalVelocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main.transform;
            if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
            controls = new InputSystem_Actions();
        }

        void OnEnable() => controls.Player.Enable();
        void OnDisable() => controls.Player.Disable();
        void OnDestroy() => controls.Dispose();

        void Update()
        {
            Vector2 input = Vector2.ClampMagnitude(controls.Player.Move.ReadValue<Vector2>(), 1f);
            float speed = controls.Player.Sprint.IsPressed() ? runSpeed : walkSpeed;

            Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            Vector3 motion = (forward * input.y + right * input.x) * speed;

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            motion.y = verticalVelocity;

            controller.Move(motion * Time.deltaTime);

            if (sprite != null && Mathf.Abs(input.x) > 0.01f) sprite.flipX = input.x < 0f;
        }
    }
}
