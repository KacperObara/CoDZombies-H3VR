using System.Collections;
using UnityEngine;
#if H3VR_IMPORTED
namespace CustomScripts
{
    public class PowerUpIndicator : MonoBehaviour
    {
        private ParticleSystem _ps;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        public void Activate(float time)
        {
            ParticleSystem.EmissionModule emission = _ps.emission;
            emission.rateOverTime = 2;

            _ps.Play(true);

            StartCoroutine(DisablePSTimer(time));
        }

        private IEnumerator DisablePSTimer(float time)
        {
            yield return new WaitForSeconds(time - 6f);

            ParticleSystem.EmissionModule emission = _ps.emission;
            emission.rateOverTime = 1;

            yield return new WaitForSeconds(6f);

            _ps.Stop(true);
        }
    }
}
#endif