using CustomScripts.Zombie;
using UnityEngine;
namespace CustomScripts
{
    public class PlayerCollider : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.position = GameReferences.Instance.PlayerHead.position;

            float yRot = GameReferences.Instance.PlayerHead.rotation.eulerAngles.y;
            Vector3 newRot = _transform.rotation.eulerAngles;
            newRot.y = yRot;
            _transform.rotation = Quaternion.Euler(newRot);
        }

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<CustomZombieController>().OnPlayerTouch();
        }

        private void OnTriggerExit(Collider other)
        {
            other.GetComponent<CustomZombieController>().OnPlayerStopTouch();
        }
    }
}