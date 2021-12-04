using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomScripts
{
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        // TODO In the future, need to decrease number of audiosources mostly playoneshot

        public List<AudioClip> FarZombieSounds;
        public List<AudioClip> CloseZombieSounds;

        public AudioSource BuySound;
        public AudioSource DrinkSound;

        public AudioSource ZombieHitSound;
        public AudioSource ZombieDeathSound;

        public AudioSource RoundStartSound;
        public AudioSource RoundEndSound;

        public AudioSource EndMusic;

        public AudioSource PlayerHitSound;

        public AudioSource PowerUpX2Sound;
        public AudioSource PowerUpDoublePointsEndSound;
        public AudioSource PowerUpNukeSound;
        public AudioSource PowerUpCarpenterSound;
        public AudioSource PowerUpInstaKillSound;
        public AudioSource PowerUpDeathMachineSound;
        public AudioSource PowerUpMaxAmmoSound;

        public AudioSource BarricadeRepairSound;

        public AudioSource PackAPunchUpgradeSound;
    }
}