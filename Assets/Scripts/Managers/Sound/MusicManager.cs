using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts.Managers.Sound
{
    public class MusicManager : MonoBehaviourSingleton<MusicManager>
    {
        private AudioSource activeAudio;
        //public AudioSource TMPMusic;

        //public List<MusicGroup> MusicGroups = new List<MusicGroup>();
        public List<AudioSource> MusicTracks;
        ////public List<AudioClip> MusicTracks;

        //private int currentGroup = 0;
        private int currentTrack = 0;

        private Coroutine musicEndCoroutine;

        public override void Awake()
        {
            base.Awake();

            GameSettings.OnSettingsChanged -= ToggleMusic;
            GameSettings.OnSettingsChanged += ToggleMusic;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GameSettings.Instance.ToggleBackgroundMusic();
            }
        }

        private void ToggleMusic()
        {
            if (GameSettings.BackgroundMusic)
            {
                PlayNextTrack();
            }
            else
            {
                StopMusic();
            }
        }

        public void PlayNextTrack()
        {
            StopMusic();

            if (!GameSettings.BackgroundMusic)
                return;

            // if (MusicGroups == null || MusicGroups.Count == 0)
            // {
            //     Debug.Log("No MusicGroup");
            //     MusicGroups = new List<MusicGroup>();
            //     MusicGroups.Add(new MusicGroup()
            //     {
            //         MusicTracks = new List<AudioSource>() {TMPMusic}
            //     });
            // }
            //
            //
            // MusicGroup musicGroup = MusicGroups[currentGroup];

            //activeAudio = musicGroup.MusicTracks[currentTrack % musicGroup.MusicTracks.Count];
            ////activeAudio.clip = MusicTracks[currentTrack];
            activeAudio = MusicTracks[currentTrack % MusicTracks.Count];
            float musicLength = activeAudio.clip.length;
            activeAudio.Play();
            musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
        }

        public void StopMusic()
        {
            if (activeAudio)
                activeAudio.Stop();

            if (musicEndCoroutine != null)
                StopCoroutine(musicEndCoroutine);
        }

        private IEnumerator OnMusicEnd(float endTime)
        {
            yield return new WaitForSeconds(endTime);

            activeAudio.Stop();
            ++currentTrack;
            PlayNextTrack();
        }

        // public void ChangeMusicGroup(int newMusicGroup)
        // {
        //     if (newMusicGroup >= MusicGroups.Count)
        //     {
        //         Debug.LogWarning("No music group with that ID!");
        //         return;
        //     }
        //
        //     currentGroup = newMusicGroup;
        // }


        // [Serializable]
        // public class MusicGroup
        // {
        //     public List<AudioSource> MusicTracks = new List<AudioSource>();
        // }

        private void OnDestroy()
        {
            GameSettings.OnSettingsChanged -= ToggleMusic;
        }
    }
}