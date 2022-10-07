#if H3VR_IMPORTED
using CODZombies.Scripts.Objects.Window;
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public abstract class ZombieController : MonoBehaviour
    {
        [HideInInspector] public Window LastInteractedWindow;

        public Transform Target;

        public abstract void Initialize(Transform newTarget);
        public abstract void InitializeSpecialType();
        public abstract void OnHit(float damage, bool headHit = false);
        public abstract void OnKill(bool awardPoints = true);
    }
}
#endif