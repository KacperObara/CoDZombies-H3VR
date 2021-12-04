using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts
{
    /// <summary>
    /// There was an error when trying to play sounds on moving objects, so I created sound pool,
    /// that spawns stationary sounds at the current positions of those objects.
    /// It works for quick sound like zombie growls, because the source of the audio is not moving
    /// </summary>
    public class SoundPool : MonoBehaviour
    {
        public static SoundPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public List<SoundPoolableObject> FreeAudio;

        public SoundPoolableObject Spawn()
        {
            if (FreeAudio.Count <= 0)
                return null;

            SoundPoolableObject soundPoolable = FreeAudio[0];
            FreeAudio.RemoveAt(0);
            return soundPoolable;
        }

        public void Despawn(SoundPoolableObject soundPoolable)
        {
            FreeAudio.Add(soundPoolable);
        }
    }
}