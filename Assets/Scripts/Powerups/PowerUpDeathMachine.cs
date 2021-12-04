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
        private Animator animator;

        public CustomItemSpawner MinigunSpawner;
        public CustomItemSpawner MagazineSpawner;

        private FVRPhysicalObject MinigunObject;
        private FVRPhysicalObject MagazineObject;

        private void Awake()
        {
            animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            if (Renderer == null) // for error debugging
            {
                Debug.LogWarning("DeathMachine spawn failed! renderer == null Tell Kodeman");
                return;
            }

            if (animator == null)
            {
                Debug.LogWarning("DeathMachine spawn failed! animator == null Tell Kodeman");
                return;
            }

            transform.position = pos;
            Renderer.enabled = true;
            animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }

        public override void ApplyModifier()
        {
            MinigunSpawner.Spawn();
            MagazineSpawner.Spawn();

            MinigunObject = MinigunSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            MinigunObject.SpawnLockable = false;
            MinigunObject.UsesGravity = false;

            MinigunObject.RootRigidbody.isKinematic = true;

            MagazineObject = MagazineSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            MagazineObject.SpawnLockable = false;
            MagazineObject.UsesGravity = false;

            MagazineObject.RootRigidbody.isKinematic = true;

            PlayerData.Instance.DeathMachinePowerUpIndicator.Activate(30f);

            StartCoroutine(DisablePowerUpDelay(30f));

            AudioManager.Instance.PowerUpDeathMachineSound.Play();

            Despawn();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.PowerUpDoublePointsEndSound.Play();

            MinigunObject.ForceBreakInteraction();
            MinigunObject.IsPickUpLocked = true;
            Destroy(MinigunObject.gameObject);

            MagazineObject.ForceBreakInteraction();
            MagazineObject.IsPickUpLocked = true;
            Destroy(MagazineObject.gameObject);
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