using System;
using System.Collections;
using UnityEngine;

namespace CustomScripts
{
    // My spider sense tells me, there is a easier way to do this
    public class QuestionMarkBlinking : MonoBehaviour
    {
        public Renderer QuestionMark1;
        public Renderer QuestionMark2;

        private float time = 0f;
        private bool emit = false;

        private float emissionValue;
        private int emissionWeight;

        private void Awake()
        {
            emissionWeight = Shader.PropertyToID("_EmissionWeight");
        }

        void Update()
        {
            if (emit)
            {
                if (time >= 1.5f)
                {
                    emit = false;
                    StartCoroutine(SmoothEmissionChange(0, 1, 1));
                    time = 0f;
                }
            }
            else
            {
                if (time >= 3.0f)
                {
                    emit = true;
                    StartCoroutine(SmoothEmissionChange(1, 0, 1));
                    time = 0f;
                }
            }

            time += Time.deltaTime;
        }

        private IEnumerator SmoothEmissionChange(float startValue, float endValue, float duration)
        {
            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                emissionValue = Mathf.Lerp(startValue, endValue, elapsed / duration);
                elapsed += Time.deltaTime;

                QuestionMark1.sharedMaterial.SetFloat(emissionWeight, emissionValue);
                QuestionMark2.sharedMaterial.SetFloat(emissionWeight, emissionValue);
                yield return null;
            }

            emissionValue = endValue;
            QuestionMark1.sharedMaterial.SetFloat(emissionWeight, emissionValue);
            QuestionMark2.sharedMaterial.SetFloat(emissionWeight, emissionValue);
        }
    }
}