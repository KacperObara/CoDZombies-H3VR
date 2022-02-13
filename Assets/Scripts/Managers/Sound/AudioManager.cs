#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomScripts
{
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        public List<AudioClip> FarZombieSounds;
        public List<AudioClip> CloseZombieSounds;
        public List<AudioClip> HellHoundsSounds;

        public AudioSource MainAudioSource;

        [Space(20)]
        public AudioClip Music;

        public AudioClip BuySound;
        public AudioClip DrinkSound;

        public AudioClip ZombieHitSound;
        public AudioClip ZombieDeathSound;

        public AudioClip HellHoundSpawnSound;
        public AudioClip HellHoundDeathSound;

        public AudioClip RoundStartSound;
        public AudioClip RoundEndSound;

        public AudioClip EndMusic;

        public AudioClip PlayerHitSound;

        public AudioClip BarricadeRepairSound;

        public override void Awake()
        {
            base.Awake();

            GameSettings.OnMusicSettingChanged += OnMusicSettingChanged;
        }

        private void OnMusicSettingChanged()
        {
            if (GameSettings.BackgroundMusic)
                PlayMusic(Music, .08f);
            else
                MainAudioSource.Stop();
        }

        public void Play(AudioClip audioClip, float volume = 1f, float delay = 0f)
        {
            if (delay != 0)
                StartCoroutine(PlayDelayed(audioClip, volume, delay));
            else
                MainAudioSource.PlayOneShot(audioClip, volume);
        }

        private IEnumerator PlayDelayed(AudioClip audioClip, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);
            MainAudioSource.PlayOneShot(audioClip, volume);
        }

        /// <summary>
        /// Used to stop old sound when playing the new one
        /// </summary>
        public void PlayMusic(AudioClip audioClip, float volume = 1f, float delay = 0f)
        {
            MainAudioSource.clip = audioClip;
            MainAudioSource.volume = volume;
            MainAudioSource.PlayDelayed(delay);
        }
    }
}
#endif