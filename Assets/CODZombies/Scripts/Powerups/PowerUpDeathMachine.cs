#if H3VR_IMPORTED
using System.Collections;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using FistVR;
using UnityEngine;

namespace CODZombies.Scripts.Powerups
{
    public class PowerUpDeathMachine : PowerUp
    {
        public AudioClip EndAudio;

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

            (_magazineObject as FVRFireArmMagazine).Load(_minigunObject as FVRFireArm);

            FVRViveHand hand = PlayerData.Instance.RightHand;
            // If players has empty right hand and is holding the grip, then auto equip minigun
            if (hand.CurrentInteractable == null &&
                hand.Grip_Button.stateDown)
            {
                Debug.Log("Player should grab minigun");
                hand.RetrieveObject(_minigunObject);
                // hand.CurrentInteractable = _minigunObject;
                // //this.m_state = FVRViveHand.HandState.GripInteracting;
                // _minigunObject.BeginInteraction(hand);
                // hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
            }

            PlayerData.Instance.DeathMachinePowerUpIndicator.Activate(30f);

            StartCoroutine(DisablePowerUpDelay(30f));

            AudioManager.Instance.Play(ApplyAudio, .5f);

            Despawn();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.Play(EndAudio, .5f);

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
#endif