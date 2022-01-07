using System.Collections;
using CustomScripts.Managers;
using FistVR;
using Sodalite.Api;
using UnityEngine;

namespace Atlas.MappingComponents.Sandbox
{
    public class CustomSosigSpawnPoint : MonoBehaviour
    {
        public SosigEnemyTemplate SosigEnemyTemplate;

        [Tooltip("Spawn when game starts?")]
        public bool SpawnOnStart = false;

        public int IFF = 1;
        public Sosig.SosigOrder SpawnState;

        public IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (SpawnOnStart) Spawn();
        }

        public void Spawn()
        {
            SosigAPI.SpawnOptions options = new SosigAPI.SpawnOptions
            {
                SpawnActivated = true,
                SpawnState = SpawnState,
                IFF = IFF,
                SpawnWithFullAmmo = true,
                EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                SosigTargetPosition = transform.position,
                SosigTargetRotation = transform.eulerAngles
            };

            Sosig spawnedSosig = SosigAPI.Spawn(SosigEnemyTemplate, options, transform.position, transform.rotation);
            ZombieManager.Instance.OnZosigSpawned(spawnedSosig);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.25f);
        }
    }
}