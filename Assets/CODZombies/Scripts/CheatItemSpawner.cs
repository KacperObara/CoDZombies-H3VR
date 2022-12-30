#if H3VR_IMPORTED

using System.Collections;
using System.Linq;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using FistVR;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CODZombies.Scripts
{
    public class CheatItemSpawner : MonoBehaviour
    {
        private void Awake()
        {
            RoundManager.OnGameStarted += TrySpawning;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.4f, 0.4f, 0.9f, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;

            Vector3 center = new Vector3(0f, 0.7f, 0.25f);
            Vector3 size = new Vector3(2.3f, 1.2f, 0.5f);
            Vector3 forward = Vector3.forward;

            Gizmos.DrawCube(center, size);
            Gizmos.DrawLine(center, center + forward * 0.5f);
        }

        public void TrySpawning()
        {
            if (GameSettings.ItemSpawnerEnabled)
                StartCoroutine(InitializeAsync());
        }

        private IEnumerator InitializeAsync()
        {
            yield return null;

            GameObject itemSpawner = IM.Prefab_ItemSpawner;
            Instantiate(itemSpawner, transform.position, transform.rotation).SetActive(true);

            Destroy(this);
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= TrySpawning;
        }
    }
}

#endif