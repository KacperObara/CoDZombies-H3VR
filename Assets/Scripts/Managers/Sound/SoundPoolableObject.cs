#if H3VR_IMPORTED
using System.Collections;
using UnityEngine;
namespace CustomScripts
{
    public class SoundPoolableObject : MonoBehaviour
    {
        public AudioSource AudioSource;
        public float DespawnTime = 4f;

        public void Initialize()
        {
            AudioSource.Play();
            StartCoroutine(DespawnDelay());
        }

        private IEnumerator DespawnDelay()
        {
            yield return new WaitForSeconds(DespawnTime);

            SoundPool.Instance.Despawn(this);
        }
    }
}
#endif