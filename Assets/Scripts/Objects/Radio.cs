using System;
using System.Collections;
using CustomScripts.Managers.Sound;
using FistVR;
using UnityEngine;

namespace CustomScripts.Objects
{
    public class Radio : MonoBehaviour, IFVRDamageable
    {
        private bool isThrottled = false;

        private AudioSource audio;

        private Coroutine musicEndCoroutine;

        private void Awake()
        {
            audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (audio.isPlaying)
                {
                    audio.Stop();
                    MusicManager.Instance.PlayNextTrack();

                    float musicLength = audio.clip.length;
                    musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
                }
                else
                {
                    audio.Play();
                    MusicManager.Instance.StopMusic();

                    if (musicEndCoroutine != null)
                        StopCoroutine(musicEndCoroutine);
                }
            }
        }

        public void Damage(Damage dam)
        {
            if (dam.Class == FistVR.Damage.DamageClass.Explosive)
                return;

            if (isThrottled)
                return;

            if (!audio)
                return;

            if (audio.isPlaying)
            {
                audio.Stop();
                MusicManager.Instance.PlayNextTrack();

                float musicLength = audio.clip.length;
                musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
            }
            else
            {
                audio.Play();
                MusicManager.Instance.StopMusic();
                if (musicEndCoroutine != null)
                    StopCoroutine(musicEndCoroutine);
            }

            StartCoroutine(Throttle());
        }

        private IEnumerator Throttle()
        {
            isThrottled = true;
            yield return new WaitForSeconds(.5f);
            isThrottled = false;
        }

        private IEnumerator OnMusicEnd(float endTimer)
        {
            yield return new WaitForSeconds(endTimer);
            audio.Stop();
            MusicManager.Instance.PlayNextTrack();
        }
    }
}