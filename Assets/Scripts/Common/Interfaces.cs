#if H3VR_IMPORTED

using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts
{
    public interface IModifier
    {
        void ApplyModifier();
    }

    public interface IPowerUp : IModifier
    {
        void Spawn(Vector3 pos);
    }

    // Needed, because I can't serialize interfaces
    public abstract class PowerUp : MonoBehaviour, IPowerUp
    {
        public abstract void ApplyModifier();
        public abstract void Spawn(Vector3 pos);
    }

    public interface IPurchasable
    {
        int PurchaseCost { get; }
    }
}

#endif