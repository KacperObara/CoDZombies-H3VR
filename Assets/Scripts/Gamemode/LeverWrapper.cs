#if H3VR_IMPORTED
using System;
using System.Collections;
using FistVR;
using UnityEngine;
using UnityEngine.Events;

namespace CustomScripts
{
    public class LeverWrapper : MonoBehaviour
    {
        public UnityEvent LeverToggleEvent;
        public UnityEvent LeverOnEvent;
        public UnityEvent LeverOffEvent;

        private TrapLever _lever;

        private bool _isOn;

        private void Awake()
        {
            _lever = GetComponent<TrapLever>();
            _lever.MessageTargets.Add(gameObject);
        }

        public void ON()
        {
            if (_isOn)
                return;
            _isOn = true;

            if (LeverToggleEvent != null)
                LeverToggleEvent.Invoke();
            if (LeverOnEvent != null)
                LeverOnEvent.Invoke();
        }

        public void OFF()
        {
            if (!_isOn)
                return;
            _isOn = false;

            if (LeverToggleEvent != null)
                LeverToggleEvent.Invoke();
            if (LeverOffEvent != null)
                LeverOffEvent.Invoke();
        }
    }
}
#endif