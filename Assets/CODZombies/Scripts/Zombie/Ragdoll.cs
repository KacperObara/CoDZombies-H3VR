#if H3VR_IMPORTED
using System.Collections;
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public class Ragdoll : MonoBehaviour
    {
        private Animator _animator;
        private CustomZombieController _controller;
        private Rigidbody[] _rbs;

        private void Awake()
        {
            _controller = GetComponent<CustomZombieController>();
            _rbs = GetComponentsInChildren<Rigidbody>();
            _animator = GetComponent<Animator>();

            _controller.OnZombieDied += ActivateRagdoll;

            DeactivateRagdoll();
        }

        private void OnDestroy()
        {
            _controller.OnZombieDied -= ActivateRagdoll;
        }

        public void DeactivateRagdoll()
        {
            foreach (Rigidbody rb in _rbs)
            {
                rb.gameObject.SetActive(false);
                rb.gameObject.SetActive(true);
                rb.isKinematic = true;
            }
        }

        /// <summary>
        /// Solution to the problem in which zombies don't reset their joints and are invisible because of it
        /// </summary>
        public void ResetRagdoll()
        {
            foreach (Rigidbody rb in _rbs)
            {
                rb.gameObject.transform.position = Vector3.zero;
                rb.gameObject.transform.rotation = Quaternion.identity;
            }
        }

        private void ActivateRagdoll(float delay)
        {
            StartCoroutine(DelayedActivate(delay));
        }

        private void EnableRagdoll()
        {
            _animator.enabled = false;
            foreach (Rigidbody rb in _rbs)
            {
                rb.isKinematic = false;
            }
        }

        private IEnumerator DelayedActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            EnableRagdoll();
        }
    }
}
#endif