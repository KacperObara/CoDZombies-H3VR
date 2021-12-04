using System;
using System.Collections;
using FistVR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CustomScripts
{
    /// <summary>
    /// Possible performance impact, lever checks and sets values every update loop.
    /// I don't know how to use observer pattern for this case, since there are no events to hook to
    /// (TrapLever has MessageTargets but how to use it?)
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

        private TrapLever lever;

        private bool isDelayDone = false;

        private bool startedHoldingThisFrame;
        private bool stoppedHoldingThisFrame;

        private bool isDown = false;

        private void Start()
        {
            lever = GetComponent<TrapLever>();
        }

        private void Update()
        {
            if (!lever.IsHeld)
            {
                startedHoldingThisFrame = false;
                isDelayDone = false;

                if (stoppedHoldingThisFrame)
                {
                    StartCoroutine(DelayedCheck2());
                }

                return;
            }

            if (!startedHoldingThisFrame)
            {
                startedHoldingThisFrame = true;
                stoppedHoldingThisFrame = false;
                StartCoroutine(DelayedCheck());
            }

            if (isDelayDone)
            {
                if (!isDown && lever.ValvePos < .1f)
                {
                    isDown = true;
                    lever.ForceBreakInteraction();
                    LeverToggleEvent?.Invoke();
                    LeverOnEvent?.Invoke();
                }

                else if (isDown && lever.ValvePos > .9f)
                {
                    isDown = false;
                    lever.ForceBreakInteraction();
                    LeverToggleEvent?.Invoke();
                    LeverOffEvent?.Invoke();
                }

                stoppedHoldingThisFrame = true;
            }
        }

        // Lever up position is 0, lever down position is 1,
        // while you're grabbing, current position is 1, and end position is 0,
        // but the moment you grab, it's still 0 for some time.
        // That's why this abomination is here
        private IEnumerator DelayedCheck()
        {
            yield return new WaitForSeconds(.1f);
            isDelayDone = true;
        }

        private IEnumerator DelayedCheck2()
        {
            yield return new WaitForSeconds(.1f);

            stoppedHoldingThisFrame = false;

            if (isDown && lever.ValvePos < .1f)
            {
                isDown = false;
                lever.ForceBreakInteraction();
                LeverToggleEvent?.Invoke();
                LeverOffEvent?.Invoke();
            }
            else if (!isDown && lever.ValvePos > .9f)
            {
                isDown = true;
                lever.ForceBreakInteraction();
                LeverToggleEvent?.Invoke();
                LeverOnEvent?.Invoke();
            }
        }

        private void OnDestroy()
        {
            LeverToggleEvent.RemoveAllListeners();
            LeverOnEvent.RemoveAllListeners();
            LeverOffEvent.RemoveAllListeners();
        }
    }
}