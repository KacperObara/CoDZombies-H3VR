#if H3VR_IMPORTED

using System.Collections;
using System.Collections.Generic;
using CustomScripts.Zombie;
using UnityEngine;

namespace CustomScripts
{
    public class PlayerCollider : MonoBehaviour
    {
        public LayerMask EnemyLayer;

        private Transform _transform;

        private List<CustomZombieController> _touchingZombies = new List<CustomZombieController>();
        private Coroutine _dealDamageCoroutine;

        private const float DamageInterval = 1.5f;
        private float _damageTimer;

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

            if (_touchingZombies.Count > 0)
            {
                if (_damageTimer <= Time.time)
                {
                    _damageTimer = Time.time + DamageInterval;
                    _touchingZombies[0].OnHitPlayer();
                    _touchingZombies.Clear(); /// Fix for an issue in which zombies are killed inside the player but still are referenced
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((EnemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                var controller = other.GetComponent<CustomZombieController>();
                if (!controller)
                    return;

                if (!_touchingZombies.Contains(controller))
                    _touchingZombies.Add(controller);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((EnemyLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                if (!other.GetComponent<CustomZombieController>())
                    return;

                _touchingZombies.Remove(other.GetComponent<CustomZombieController>());
            }
        }
    }
}
#endif