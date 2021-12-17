using UnityEngine;
namespace CustomScripts.Player
{
    public class PlayerColliderSmall : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<IModifier>().ApplyModifier();
        }
    }
}