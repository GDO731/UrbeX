using Assets.Game.Scripts.Constants;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Game.Scripts.Cam
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float lookHeight = 0.9f;
        [SerializeField] float smoothTime = 0.15f;

        [SerializeField] float distance = 10f;
        [SerializeField] Vector3 rotation = new Vector3(40f, 0f, 0f);
        [SerializeField] float minRotationX = 15f;
        [SerializeField] float maxRotationX = 80f;
        [SerializeField] float rotateSpeed = 0.2f;
        [SerializeField] float rotateStep = 45f;
        [SerializeField] float rotateSmoothTime = 0.1f;

        [SerializeField] float zoom = 1f;
        [SerializeField] float minZoom = 0.5f;
        [SerializeField] float maxZoom = 2f;
        [SerializeField] float zoomStep = 0.1f;

        [SerializeField] float panSpeed = 0.01f;
        [SerializeField] float maxPan = 6f;
        [SerializeField] float panReturnTime = 0.3f;

        Vector3 velocity;
        Vector3 pan;
        Vector3 panVelocity;
        Vector3 targetRotation;
        Vector3 rotationVelocity;

        void Start()
        {
            if (target == null) target = GameObject.FindGameObjectWithTag(TagConstants.PlayerTag).transform;
            targetRotation = rotation;
        }

        void OnValidate()
        {
            targetRotation = rotation;
        }

        void LateUpdate()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll != 0f) zoom = Mathf.Clamp(zoom - Mathf.Sign(scroll) * zoomStep, minZoom, maxZoom);

                if (mouse.rightButton.isPressed)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    targetRotation.y += delta.x * rotateSpeed;
                    targetRotation.x = Mathf.Clamp(targetRotation.x - delta.y * rotateSpeed, minRotationX, maxRotationX);
                }

                if (mouse.middleButton.isPressed)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                    pan -= (right * delta.x + forward * delta.y) * panSpeed * zoom;
                    pan = Vector3.ClampMagnitude(pan, maxPan);
                }
                else
                {
                    pan = Vector3.SmoothDamp(pan, Vector3.zero, ref panVelocity, panReturnTime);
                }
            }

            if (keyboard != null)
            {
                if (keyboard.qKey.wasPressedThisFrame) targetRotation.y -= rotateStep;
                if (keyboard.eKey.wasPressedThisFrame) targetRotation.y += rotateStep;
            }

            rotation.x = Mathf.SmoothDamp(rotation.x, targetRotation.x, ref rotationVelocity.x, rotateSmoothTime);
            rotation.y = Mathf.SmoothDampAngle(rotation.y, targetRotation.y, ref rotationVelocity.y, rotateSmoothTime);
            rotation.z = Mathf.SmoothDamp(rotation.z, targetRotation.z, ref rotationVelocity.z, rotateSmoothTime);

            Vector3 offset = Quaternion.Euler(rotation.x, rotation.y, 0f) * Vector3.back * (distance * zoom);
            Vector3 goal = target.position + pan + offset;
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
            transform.rotation = Quaternion.LookRotation(Vector3.up * lookHeight - offset) * Quaternion.Euler(0f, 0f, rotation.z);
        }
    }
}
