using UnityEngine;

namespace CustomScripts.Zombie
{
    public abstract class ZombieController : MonoBehaviour
    {
        [HideInInspector] public Window LastInteractedWindow;

        public Transform Target;

        public abstract void Initialize(Transform newTarget);
        public abstract void OnHit(float damage, bool headHit = false);
        public abstract void OnHitPlayer();

        public abstract void ChangeTarget(Transform newTarget);
    }
}