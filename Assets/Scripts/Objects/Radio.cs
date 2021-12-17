using System.Collections;
using CustomScripts.Managers.Sound;
using FistVR;
using UnityEngine;
namespace CustomScripts.Objects
{
    public class Radio : MonoBehaviour, IFVRDamageable
    {
        private AudioSource _audio;
        private bool _isThrottled;

        private Coroutine _musicEndCoroutine;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_audio.isPlaying)
                {
                    _audio.Stop();
                    MusicManager.Instance.PlayNextTrack();

                    var musicLength = _audio.clip.length;
                    _musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
                }
                else
                {
                    _audio.Play();
                    MusicManager.Instance.StopMusic();

                    if (_musicEndCoroutine != null)
                        StopCoroutine(_musicEndCoroutine);
                }
            }
        }

        public void Damage(Damage dam)
        {
            if (dam.Class == FistVR.Damage.DamageClass.Explosive)
                return;

            if (_isThrottled)
                return;

            if (!_audio)
                return;

            if (_audio.isPlaying)
            {
                _audio.Stop();
                MusicManager.Instance.PlayNextTrack();

                var musicLength = _audio.clip.length;
                _musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
            }
            else
            {
                _audio.Play();
                MusicManager.Instance.StopMusic();
                if (_musicEndCoroutine != null)
                    StopCoroutine(_musicEndCoroutine);
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
            _audio.Stop();
            MusicManager.Instance.PlayNextTrack();
        }
    }
}