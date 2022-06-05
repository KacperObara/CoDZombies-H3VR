#if H3VR_IMPORTED

using UnityEngine;

namespace CustomScripts.Player
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