#if H3VR_IMPORTED

using CODZombies.Scripts.Common;
using UnityEngine;

namespace CODZombies.Scripts.Player
{
    public class PlayerColliderSmall : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IModifier>() != null)
                other.GetComponent<IModifier>().ApplyModifier();
        }
    }
}
#endif