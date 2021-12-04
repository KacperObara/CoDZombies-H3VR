using System;
using UnityEngine;

namespace CustomScripts.Zombie
{
    public class ZosigTrigger : MonoBehaviour
    {
        private ZosigZombieController zosigController;

        public void Initialize(ZosigZombieController controller)
        {
            zosigController = controller;
        }

        private void OnTriggerEnter(Collider other)
        {
            zosigController.OnTriggerEntered(other);
        }
    }
}