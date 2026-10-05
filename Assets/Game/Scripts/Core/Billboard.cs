using UnityEngine;

namespace Assets.Game.Scripts.Core
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] bool faceCamera;

        Transform cam;

        void Awake() => cam = Camera.main.transform;

        void LateUpdate()
        {
            transform.rotation = faceCamera
                ? Quaternion.Euler(0f, cam.eulerAngles.y, 0f)
                : cam.rotation;
        }
    }
}
