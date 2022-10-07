#if H3VR_IMPORTED
using CODZombies.Scripts.Objects.Window;
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public class ZombieSpawner : MonoBehaviour
    {
        [Tooltip("If left empty, spawned zombies will go straight for the player")]
        public Window WindowWaypoint;
    }
}
#endif