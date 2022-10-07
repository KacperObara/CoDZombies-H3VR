#if H3VR_IMPORTED
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public class ZosigTrigger : MonoBehaviour
    {
        private ZosigZombieController _zosigController;

        private void OnTriggerEnter(Collider other)
        {
            _zosigController.OnTriggerEntered(other);
        }

        public void Initialize(ZosigZombieController controller)
        {
            _zosigController = controller;
        }

        private void OnTriggerExit(Collider other)
        {
            _zosigController.OnTriggerExited(other);
        }
    }
}
#endif