using UnityEngine;

namespace Assets.Game.Scripts.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        [SerializeField] AudioSource defaultAudioSource;

        private void Awake()
        {
            instance = this;
        }

        public void PlayClip(AudioClip audioClip ,AudioSource audioSource = null)
        {
            if ((audioSource == null))
            {
                audioSource = defaultAudioSource;
            }
            audioSource.Stop();
            if (audioClip == null) return;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void PlayClip(AudioClip audioClip)
        {
            PlayClip(audioClip, null);
        }
    }
}
