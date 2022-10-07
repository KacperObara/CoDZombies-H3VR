#if H3VR_IMPORTED
using CODZombies.Scripts.Common;

namespace CODZombies.Scripts.Managers.Sound
{
    public class MusicManager : MonoBehaviourSingleton<MusicManager>
    {
        // //public AudioSource TMPMusic;
        //
        // //public List<MusicGroup> MusicGroups = new List<MusicGroup>();
        // public List<AudioSource> MusicTracks;
        // private AudioSource _activeAudio;
        // ////public List<AudioClip> MusicTracks;
        //
        // //private int currentGroup = 0;
        // private int _currentTrack;
        //
        // private Coroutine _musicEndCoroutine;
        //
        // public override void Awake()
        // {
        //     base.Awake();
        //
        //     GameSettings.OnSettingsChanged -= ToggleMusic;
        //     GameSettings.OnSettingsChanged += ToggleMusic;
        // }
        //
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Q))
        //     {
        //         GameSettings.Instance.ToggleBackgroundMusic();
        //     }
        // }
        //
        // // public void ChangeMusicGroup(int newMusicGroup)
        // // {
        // //     if (newMusicGroup >= MusicGroups.Count)
        // //     {
        // //         Debug.LogWarning("No music group with that ID!");
        // //         return;
        // //     }
        // //
        // //     currentGroup = newMusicGroup;
        // // }
        //
        //
        // // [Serializable]
        // // public class MusicGroup
        // // {
        // //     public List<AudioSource> MusicTracks = new List<AudioSource>();
        // // }
        //
        // private void OnDestroy()
        // {
        //     GameSettings.OnSettingsChanged -= ToggleMusic;
        // }
        //
        // private void ToggleMusic()
        // {
        //     if (GameSettings.BackgroundMusic)
        //     {
        //         PlayNextTrack();
        //     }
        //     else
        //     {
        //         StopMusic();
        //     }
        // }
        //
        // public void PlayNextTrack()
        // {
        //     StopMusic();
        //
        //     if (!GameSettings.BackgroundMusic)
        //         return;
        //
        //     // if (MusicGroups == null || MusicGroups.Count == 0)
        //     // {
        //     //     Debug.Log("No MusicGroup");
        //     //     MusicGroups = new List<MusicGroup>();
        //     //     MusicGroups.Add(new MusicGroup()
        //     //     {
        //     //         MusicTracks = new List<AudioSource>() {TMPMusic}
        //     //     });
        //     // }
        //     //
        //     //
        //     // MusicGroup musicGroup = MusicGroups[currentGroup];
        //
        //     //activeAudio = musicGroup.MusicTracks[currentTrack % musicGroup.MusicTracks.Count];
        //     ////activeAudio.clip = MusicTracks[currentTrack];
        //     _activeAudio = MusicTracks[_currentTrack % MusicTracks.Count];
        //     var musicLength = _activeAudio.clip.length;
        //     _activeAudio.Play();
        //     _musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));
        // }
        //
        // public void StopMusic()
        // {
        //     if (_activeAudio)
        //         _activeAudio.Stop();
        //
        //     if (_musicEndCoroutine != null)
        //         StopCoroutine(_musicEndCoroutine);
        // }
        //
        // private IEnumerator OnMusicEnd(float endTime)
        // {
        //     yield return new WaitForSeconds(endTime);
        //
        //     _activeAudio.Stop();
        //     ++_currentTrack;
        //     PlayNextTrack();
        // }
    }
}
#endif