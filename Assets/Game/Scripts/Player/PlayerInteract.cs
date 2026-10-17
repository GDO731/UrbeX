using Assets.Game.Scripts.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Game.Scripts.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        InputSystem_Actions controls;
        Camera cam;

        void Awake()
        {
            cam = Camera.main;
            controls = new InputSystem_Actions();
        }

        void OnEnable() => controls.Player.Enable();
        void OnDisable() => controls.Player.Disable();
        void OnDestroy() => controls.Dispose();

        void Update()
        {
            if (!controls.Player.Interact.WasPressedThisFrame()) return;

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable == null) return;
            if (Vector3.Distance(transform.position, interactable.transform.position) > interactable.GetRange()) return;

            interactable.Interact();
        }
    }
}