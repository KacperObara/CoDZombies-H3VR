#if H3VR_IMPORTED
using CODZombies.Scripts.Common;
using UnityEngine;

namespace CODZombies.Scripts.Gamemode
{
    public class GameStart : MonoBehaviourSingleton<GameStart>
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
#endif