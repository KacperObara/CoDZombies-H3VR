#if H3VR_IMPORTED

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomScripts
{
    public class MysteryBoxMover : MonoBehaviour
    {
        public List<Transform> SpawnPoints;

        [Range(0, 100)] public float TeleportChance = 20f;
        public int SafeRollsProvided = 3;

        public AudioSource ByeByeSound;

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

        public void StartTeleportAnim()
        {
            ByeByeSound.Play();

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