using System;
using System.Collections;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class PowerUpIndicator : MonoBehaviour
    {
        private ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Activate(float time)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 2;

            ps.Play(true);

            StartCoroutine(DisablePSTimer(time));
        }

        private IEnumerator DisablePSTimer(float time)
        {
            yield return new WaitForSeconds(time - 6f);

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 1;

            yield return new WaitForSeconds(6f);

            ps.Stop(true);
        }
    }
}