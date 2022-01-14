#if H3VR_IMPORTED
using System;
using System.Collections;
using CustomScripts.Player;
using UnityEngine;
namespace CustomScripts
{
    public class PowerUpDoublePoints : PowerUp
    {
        public static Action PickedUpEvent;

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
            PlayerData.Instance.MoneyModifier = 2f;
            PlayerData.Instance.DoublePointsPowerUpIndicator.Activate(30f);
            StartCoroutine(DisablePowerUpDelay(30f));

            AudioManager.Instance.PowerUpX2Sound.Play();

            if (PickedUpEvent != null)
                PickedUpEvent.Invoke();

            Despawn();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.PowerUpDoublePointsEndSound.Play();
            PlayerData.Instance.MoneyModifier = 1f;
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