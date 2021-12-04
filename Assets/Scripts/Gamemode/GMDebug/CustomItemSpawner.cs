using System.Collections;
using FistVR;
using UnityEngine;

namespace CustomScripts.Gamemode.GMDebug
{
    public class CustomItemSpawner : ComponentProxy
    {
        public string ObjectId;
        public bool SpawnOnLoad;

        public GameObject SpawnedObject;

        public override void InitializeComponent()
        {
            if (SpawnOnLoad) Spawn();
        }

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

           GameObject go = Instantiate(callback, transform.position, transform.rotation,
               ObjectReferences.CustomScene.transform);
           go.SetActive(true);

           SpawnedObject = go;
        }
    }
}