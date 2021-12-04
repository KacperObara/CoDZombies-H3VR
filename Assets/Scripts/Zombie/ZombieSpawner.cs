using UnityEngine;

namespace CustomScripts.Zombie
{
    public class ZombieSpawner : MonoBehaviour
    {
        [Tooltip("If left empty, spawned zombies will go straight for the player")]
        public Window WindowWaypoint;
    }
}