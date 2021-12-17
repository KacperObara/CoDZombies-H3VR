#if H3VR_IMPORTED
using UnityEngine;
namespace CustomScripts.Zombie
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
    }
}
#endif