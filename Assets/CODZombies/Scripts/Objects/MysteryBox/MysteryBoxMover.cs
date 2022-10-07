#if H3VR_IMPORTED

using System.Collections;
using System.Collections.Generic;
using CODZombies.Scripts.Managers.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CODZombies.Scripts.Objects.MysteryBox
{
    public class MysteryBoxMover : MonoBehaviour
    {
        public List<Transform> SpawnPoints;

        [Range(0, 100)] public float TeleportChance = 20f;
        public int SafeRollsProvided = 3;

        public AudioClip TeddyBearSound;
        public AudioClip SecretTeddyBearSound;

        public Transform TeddyBear;

        [HideInInspector] public Transform CurrentPos;
        [HideInInspector] public int CurrentRoll = 0;
        private Animator _animator;

        private MysteryBox _mysteryBox;
        private Transform _parent;

        private void Awake()
        {
            _parent = transform.parent;
            _animator = _parent.GetComponent<Animator>();
            _mysteryBox = GetComponent<MysteryBox>();
        }

        private void Start()
        {
            Teleport(true);
        }

        public void Teleport(bool firstTime = false)
        {
            Transform newPos = SpawnPoints[Random.Range(0, SpawnPoints.Count)];

            if (!firstTime)
            {
                SpawnPoints.Add(CurrentPos);
            }

            CurrentPos = newPos;
            SpawnPoints.Remove(newPos); // Exclude current transform from randomization

            _parent.transform.position = newPos.position;
            _parent.transform.rotation = newPos.rotation;

            CurrentRoll = 0;
            _mysteryBox.InUse = false;
        }

        public bool TryTeleport()
        {
            if (CurrentRoll <= SafeRollsProvided)
                return false;

            return (Random.Range(0, 100) <= TeleportChance);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartTeleportAnim();
            }
        }

        public void StartTeleportAnim()
        {
            int secretTeddyChance = Random.Range(0, 5801);
            GameObject teddy;

            if (secretTeddyChance == 0)
            {
                teddy = TeddyBear.GetChild(1).gameObject;

                AudioManager.Instance.Play(SecretTeddyBearSound);
            }
            else
            {
                teddy = TeddyBear.GetChild(0).gameObject;

                AudioManager.Instance.Play(TeddyBearSound);
            }
            TeddyBear.GetComponent<Animator>().Play("Activation");
            teddy.SetActive(true);

            StartCoroutine(DelayedAnimation(teddy));
        }

        private IEnumerator DelayedAnimation(GameObject teddy)
        {
            yield return new WaitForSeconds(3f);

            teddy.SetActive(false);

            yield return new WaitForSeconds(1.2f);

            _animator.Play("Teleport");
            StartCoroutine(DelayedTeleport());
        }

        private IEnumerator DelayedTeleport()
        {
            yield return new WaitForSeconds(4.2f);
            Teleport();
        }
    }
}
#endif