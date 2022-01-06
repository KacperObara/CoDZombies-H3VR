#if H3VR_IMPORTED
using System.Collections;
using FistVR;
using UnityEngine;
using UnityEngine.Events;

namespace CustomScripts
{
    /// <summary>
    /// Possible performance impact, lever checks and sets values every update loop.
    /// I don't know how to use observer pattern for this case, since there are no events to hook to
    /// (TrapLever has MessageTargets but how to use i)
    /// (FVRLever might be a better lever script, but it throws FVRUpdate null errors)
    ///
    /// Edit:
    /// https://github.com/WFIOST/HADES.H3VR/blob/5ffff8153cb5b9f891d474c8863177b592c8857b/HADES.Core/src/Enhancements/EnhancedMovement.cs#L71
    /// turning methods into events
    /// </summary>
    public class LeverWrapper : MonoBehaviour
    {
        public UnityEvent LeverToggleEvent;
        public UnityEvent LeverOnEvent;
        public UnityEvent LeverOffEvent;

        private bool _isDelayDone;

        private bool _isDown;

        private TrapLever _lever;

        private bool _startedHoldingThisFrame;
        private bool _stoppedHoldingThisFrame;

        private void Start()
        {
            _lever = GetComponent<TrapLever>();
        }

        private void Update()
        {
            if (!_lever.IsHeld)
            {
                _startedHoldingThisFrame = false;
                _isDelayDone = false;

                if (_stoppedHoldingThisFrame)
                {
                    StartCoroutine(DelayedCheck2());
                }

                return;
            }

            if (!_startedHoldingThisFrame)
            {
                _startedHoldingThisFrame = true;
                _stoppedHoldingThisFrame = false;
                StartCoroutine(DelayedCheck());
            }

            if (_isDelayDone)
            {
                if (!_isDown && _lever.ValvePos < .1f)
                {
                    _isDown = true;
                    _lever.ForceBreakInteraction();

                    if (LeverToggleEvent != null)
                        LeverToggleEvent.Invoke();
                    if (LeverOnEvent != null)
                        LeverOnEvent.Invoke();
                }

                else if (_isDown && _lever.ValvePos > .9f)
                {
                    _isDown = false;
                    _lever.ForceBreakInteraction();

                    if (LeverToggleEvent != null)
                        LeverToggleEvent.Invoke();
                    if (LeverOffEvent != null)
                        LeverOffEvent.Invoke();
                }

                _stoppedHoldingThisFrame = true;
            }
        }

        private void OnDestroy()
        {
            LeverToggleEvent.RemoveAllListeners();
            LeverOnEvent.RemoveAllListeners();
            LeverOffEvent.RemoveAllListeners();
        }

        // Lever up position is 0, lever down position is 1,
        // while you're grabbing, current position is 1, and end position is 0,
        // but the moment you grab, it's still 0 for some time.
        // That's why this abomination is here
        private IEnumerator DelayedCheck()
        {
            yield return new WaitForSeconds(.1f);
            _isDelayDone = true;
        }

        private IEnumerator DelayedCheck2()
        {
            yield return new WaitForSeconds(.1f);

            _stoppedHoldingThisFrame = false;

            if (_isDown && _lever.ValvePos < .1f)
            {
                _isDown = false;
                _lever.ForceBreakInteraction();
                if (LeverToggleEvent != null)
                    LeverToggleEvent.Invoke();
                if (LeverOffEvent != null)
                    LeverOffEvent.Invoke();
            }
            else if (!_isDown && _lever.ValvePos > .9f)
            {
                _isDown = true;
                _lever.ForceBreakInteraction();

                if (LeverToggleEvent != null)
                    LeverToggleEvent.Invoke();
                if (LeverOnEvent != null)
                    LeverOnEvent.Invoke();
            }
        }
    }
}
#endif