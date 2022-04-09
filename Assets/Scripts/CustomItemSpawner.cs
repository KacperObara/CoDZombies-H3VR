#if H3VR_IMPORTED

using FistVR;
using UnityEngine;
namespace CustomScripts.Gamemode.GMDebug
{
    public class CustomItemSpawner : MonoBehaviour
    {
        public string ObjectId;

        [HideInInspector] public GameObject SpawnedObject;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 0.0f, 0.6f, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.zero, 0.1f);
        }

        public void Spawn()
        {
            FVRObject obj = IM.OD[ObjectId];
            GameObject callback = obj.GetGameObject();

            GameObject go = Instantiate(callback, transform.position, transform.rotation);
            go.SetActive(true);

            SpawnedObject = go;
        }
    }
}

#endif