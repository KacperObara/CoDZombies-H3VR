#if H3VR_IMPORTED
using System.Collections;
using UnityEngine;
namespace CustomScripts
{
    // My spider sense tells me, there is a easier way to do this
    public class QuestionMarkBlinking : MonoBehaviour
    {
        public Renderer QuestionMark1;
        public Renderer QuestionMark2;

        private float _emissionValue;
        private int _emissionWeight;
        private bool _emit;

        private float _time;

        private void Awake()
        {
            _emissionWeight = Shader.PropertyToID("_EmissionWeight");
        }

        void Update()
        {
            if (_emit)
            {
                if (_time >= 1.5f)
                {
                    _emit = false;
                    StartCoroutine(SmoothEmissionChange(0, 1, 1));
                    _time = 0f;
                }
            }
            else
            {
                if (_time >= 3.0f)
                {
                    _emit = true;
                    StartCoroutine(SmoothEmissionChange(1, 0, 1));
                    _time = 0f;
                }
            }

            _time += Time.deltaTime;
        }

        private IEnumerator SmoothEmissionChange(float startValue, float endValue, float duration)
        {
            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                _emissionValue = Mathf.Lerp(startValue, endValue, elapsed / duration);
                elapsed += Time.deltaTime;

                QuestionMark1.sharedMaterial.SetFloat(_emissionWeight, _emissionValue);
                QuestionMark2.sharedMaterial.SetFloat(_emissionWeight, _emissionValue);
                yield return null;
            }

            _emissionValue = endValue;
            QuestionMark1.sharedMaterial.SetFloat(_emissionWeight, _emissionValue);
            QuestionMark2.sharedMaterial.SetFloat(_emissionWeight, _emissionValue);
        }
    }
}
#endif