#if H3VR_IMPORTED
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomScripts
{
    /// <summary>
    /// There was an error when trying to play sounds on moving objects, so I created sound pool,
    /// that spawns stationary sounds at the current positions of those objects.
    /// It works for quick sound like zombie growls, because the source of the audio is not moving
    /// </summary>
    public class SoundPool : MonoBehaviour
    {
        public List<SoundPoolableObject> PooledAudioSources;
        public static SoundPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public SoundPoolableObject Spawn()
        {
            if (PooledAudioSources.Count <= 0)
                return null;

            SoundPoolableObject soundPoolable = PooledAudioSources[0];
            PooledAudioSources.RemoveAt(0);
            return soundPoolable;
        }

        public void Despawn(SoundPoolableObject soundPoolable)
        {
            PooledAudioSources.Add(soundPoolable);
        }
    }
}
#endif