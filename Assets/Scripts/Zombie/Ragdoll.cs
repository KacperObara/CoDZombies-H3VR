using System.Collections;
using UnityEngine;
namespace CustomScripts.Zombie
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
            _controller.OnZombieInitialize += DeactivateRagdoll;
        }

        private void OnDestroy()
        {
            _controller.OnZombieDied -= ActivateRagdoll;
            _controller.OnZombieInitialize -= DeactivateRagdoll;
        }

        private void DeactivateRagdoll()
        {
            _animator.enabled = true;
            foreach (Rigidbody rb in _rbs)
            {
                rb.isKinematic = true;
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

            StartCoroutine(DampenFall());
        }

        private IEnumerator DampenFall()
        {
            yield return new WaitForSeconds(.1f);
            foreach (Rigidbody rb in _rbs)
            {
                rb.velocity = Vector3.zero;
                rb.ResetInertiaTensor();
            }
        }

        private IEnumerator DelayedActivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            EnableRagdoll();
        }
    }
}