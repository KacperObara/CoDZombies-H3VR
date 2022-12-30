#if H3VR_IMPORTED
using System;
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

        private void OnTriggerStay(Collider other)
        {
            _zosigController.OnTriggerStayed(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _zosigController.OnTiggerExited(other);
        }

        public void Initialize(ZosigZombieController controller)
        {
            _zosigController = controller;
        }
    }
}
#endif