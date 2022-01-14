#if H3VR_IMPORTED

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomScripts.Gamemode.GMDebug
{
    public class CheatItemSpawner : MonoBehaviour
    {
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

        public void Spawn()
        {
            StartCoroutine(InitializeAsync());
        }

        private IEnumerator InitializeAsync()
        {
            yield return null;

            GameObject[] rootGameObjects = SceneManager.GetSceneByName("ModBlank_Simple").GetRootGameObjects();
            GameObject itemSpawner = rootGameObjects.First(x => x.name == "ItemSpawner");

            Instantiate(itemSpawner, transform.position, transform.rotation).SetActive(true);

            Destroy(this);
        }
    }
}

#endif