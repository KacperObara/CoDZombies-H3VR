#if H3VR_IMPORTED

using System;
using System.Collections;
using CustomScripts.Managers.Sound;
using FistVR;
using UnityEngine;
namespace CustomScripts.Objects
{
    public class Radio : MonoBehaviour, IFVRDamageable
    {
        public AudioClip Song;

        private bool _isThrottled;
        private bool _isPlaying;

        private Coroutine _musicEndCoroutine;

        private void Awake()
        {
            GameSettings.OnMusicSettingChanged += OnMusicSettingsChanged;
        }

        private void OnMusicSettingsChanged()
        {
            _isPlaying = false;
            if (_musicEndCoroutine != null)
                StopCoroutine(_musicEndCoroutine);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Damage dam = new Damage();
                Damage(dam);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                GameSettings.Instance.ToggleBackgroundMusic();
            }
        }

        public void Damage(Damage dam)
        {
            if (dam.Class == FistVR.Damage.DamageClass.Explosive)
                return;

            if (_isThrottled)
                return;

            if (_isPlaying)
            {
                _isPlaying = false;

                AudioManager.Instance.MusicAudioSource.Stop();

                if (GameSettings.BackgroundMusic)
                {
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.Music, .08f);
                }

                if (_musicEndCoroutine != null)
                    StopCoroutine(_musicEndCoroutine);
            }
            else
            {
                _isPlaying = true;

                var musicLength = Song.length;
                _musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));

                AudioManager.Instance.PlayMusic(Song, 0.095f);
            }

            StartCoroutine(Throttle());
        }

        private IEnumerator Throttle()
        {
            _isThrottled = true;
            yield return new WaitForSeconds(.5f);
            _isThrottled = false;
        }

        private IEnumerator OnMusicEnd(float endTimer)
        {
            yield return new WaitForSeconds(endTimer);

            _isPlaying = false;

            if (GameSettings.BackgroundMusic)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.Music, .08f);
            }
        }

        private void OnDestroy()
        {
            GameSettings.OnMusicSettingChanged -= OnMusicSettingsChanged;
        }
    }
}
#endif