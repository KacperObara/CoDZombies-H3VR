#if H3VR_IMPORTED
using CODZombies.Scripts.Objects.Window;
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public class ZombieSpawner : MonoBehaviour
    {
        [Header("If left empty, spawned zombies will go straight to the player")]
        public Window WindowWaypoint;
    }
}
#endif