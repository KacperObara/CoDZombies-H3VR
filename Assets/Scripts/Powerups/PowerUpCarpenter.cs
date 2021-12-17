#if H3VR_IMPORTED
using System.Collections;
using UnityEngine;
namespace CustomScripts.Powerups
{
    public class PowerUpCarpenter : PowerUp
    {
        public MeshRenderer Renderer;
        private Animator _animator;

        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            if (Renderer == null) // for error debugging
            {
                Debug.LogWarning("CarpenterPowerUp spawn failed! renderer == null Tell Kodeman");
                return;
            }

            if (_animator == null)
            {
                Debug.LogWarning("CarpenterPowerUp spawn failed! animator == null Tell Kodeman");
                return;
            }

            transform.position = pos;
            Renderer.enabled = true;
            _animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            StartCoroutine(DelayedApply());

            AudioManager.Instance.PowerUpCarpenterSound.Play();

            Despawn();
        }

        private IEnumerator DelayedApply()
        {
            yield return new WaitForSeconds(1f);

            foreach (var window in GameReferences.Instance.Windows)
            {
                window.RepairAll();
            }

            AudioManager.Instance.BarricadeRepairSound.Play();
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