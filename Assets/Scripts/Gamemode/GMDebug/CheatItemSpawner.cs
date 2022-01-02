#if H3VR_IMPORTED

using System.Collections;
using UnityEngine;
namespace CustomScripts.Gamemode.GMDebug
{
    public class CheatItemSpawner : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            //TODO: fix gizmos
            // WurstMod.Shared.Extensions.GenericGizmoCube(new Color(0.4f, 0.4f, 0.9f, 0.5f), new Vector3(0.0f, 0.7f, 0.25f), new Vector3(2.3f, 1.2f, 0.5f), Vector3.forward, this.transform);
        }
        public void Spawn()
        {
            StartCoroutine(InitializeAsync());
        }

        private IEnumerator InitializeAsync()
        {
            yield return null;

            //TODO: This entire part need to be supplimented
            // GameObject spawner = Instantiate(ObjectReferences.ItemSpawnerDonor, ObjectReferences.CustomScene.transform);
            // spawner.transform.position = transform.position;
            // spawner.transform.localEulerAngles = transform.localEulerAngles;
            // spawner.SetActive(true);
            // Destroy(this);

        }
    }
}

#endif