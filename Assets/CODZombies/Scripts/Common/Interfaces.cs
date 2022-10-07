#if H3VR_IMPORTED

using CODZombies.Scripts.Zombie;
using UnityEngine;

namespace CODZombies.Scripts.Common
{
    // Juggernog, DoubleTap etc.
    public interface IModifier
    {
        void ApplyModifier();
    }

    // Insta kill, double points etc.
    public interface IPowerUp : IModifier
    {
        void Spawn(Vector3 pos);
    }

    // Needed, because I can't serialize interfaces
    public abstract class PowerUp : MonoBehaviour, IPowerUp
    {
        public abstract void ApplyModifier();
        public abstract void Spawn(Vector3 pos);

        public AudioClip ApplyAudio;
    }

    // Shops, doors, traps, teleports etc.
    public interface IPurchasable
    {
        int PurchaseCost { get; }
        bool IsOneTimeOnly { get; }
        bool AlreadyBought { get; }
    }

    public interface IRequiresPower
    {
        bool IsPowered { get; }
    }

    public interface ITrap
    {
        void OnEnemyEntered(ZombieController controller);
    }
}

#endif