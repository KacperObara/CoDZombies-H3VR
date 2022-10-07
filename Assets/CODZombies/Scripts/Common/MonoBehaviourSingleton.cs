#if H3VR_IMPORTED

using UnityEngine;

namespace CODZombies.Scripts.Common
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour
        where T : Component
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

#endif