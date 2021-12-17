using System.Collections;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Player;
using FistVR;
using UnityEngine;
namespace CustomScripts.Powerups
{
    public class PowerUpDeathMachine : PowerUp
    {
        public MeshRenderer Renderer;

        public CustomItemSpawner MinigunSpawner;
        public CustomItemSpawner MagazineSpawner;
        private Animator _animator;
        private FVRPhysicalObject _magazineObject;

        private FVRPhysicalObject _minigunObject;

        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            if (Renderer == null) // for error debugging
            {
                Debug.LogWarning("DeathMachine spawn failed! renderer == null Tell Kodeman");
                return;
            }

            if (_animator == null)
            {
                Debug.LogWarning("DeathMachine spawn failed! animator == null Tell Kodeman");
                return;
            }

            transform.position = pos;
            Renderer.enabled = true;
            _animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            MinigunSpawner.Spawn();
            MagazineSpawner.Spawn();

            _minigunObject = MinigunSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            _minigunObject.SpawnLockable = false;
            _minigunObject.UsesGravity = false;

            _minigunObject.RootRigidbody.isKinematic = true;

            _magazineObject = MagazineSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            _magazineObject.SpawnLockable = false;
            _magazineObject.UsesGravity = false;

            _magazineObject.RootRigidbody.isKinematic = true;

            PlayerData.Instance.DeathMachinePowerUpIndicator.Activate(30f);

            StartCoroutine(DisablePowerUpDelay(30f));

            AudioManager.Instance.PowerUpDeathMachineSound.Play();

            Despawn();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.PowerUpDoublePointsEndSound.Play();

            _minigunObject.ForceBreakInteraction();
            _minigunObject.IsPickUpLocked = true;
            Destroy(_minigunObject.gameObject);

            _magazineObject.ForceBreakInteraction();
            _magazineObject.IsPickUpLocked = true;
            Destroy(_magazineObject.gameObject);
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