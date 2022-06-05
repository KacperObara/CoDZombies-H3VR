#if H3VR_IMPORTED
using System.Collections;
using CustomScripts.Managers;
using UnityEngine;
namespace CustomScripts
{
    public class PowerUpNuke : PowerUp
    {
        public MeshRenderer Renderer;
        private Animator _animator;

        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            transform.position = pos;
            Renderer.enabled = true;
            _animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            StartCoroutine(DelayedKillAll());

            AudioManager.Instance.Play(ApplyAudio, .5f);

            Despawn();
        }

        // Partially to make better visuals, partially for better performance (not everything at once)
        private IEnumerator DelayedKillAll()
        {
            // Reversed loop, because I'm deleting elements from this array
            for (int i = ZombieManager.Instance.ExistingZombies.Count - 1; i >= 0; i--)
            {
                // If someone kills zombie during this delay, method breaks without this check
                if (ZombieManager.Instance.ExistingZombies.Count > i) // temporary solution, until I think of better one
                {
                    ZombieManager.Instance.ExistingZombies[i].OnHit(9999);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            yield return null;
        }

        private void Despawn()
        {
            transform.position += new Vector3(0, -1000f, 0);
        }

        private IEnumerator DespawnDelay()
        {
            yield return new WaitForSeconds(15f);

            for (int i = 0; i < 5; i++)
            {
                Renderer.enabled = false;
                yield return new WaitForSeconds(.3f);
                Renderer.enabled = true;
                yield return new WaitForSeconds(.7f);
            }

            Despawn();
        }
    }
}
#endif