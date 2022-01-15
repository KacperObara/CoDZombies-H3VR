using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;
using UnityEngine.Events;

namespace CustomScripts
{
    public class PlayerTrigger : MonoBehaviour
    {
        public UnityEvent OnEnter;
        public UnityEvent OnExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerCollider>())
            {
                if (OnEnter != null)
                    OnEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerCollider>())
            {
                if (OnExit != null)
                    OnExit.Invoke();
            }
        }
    }
}