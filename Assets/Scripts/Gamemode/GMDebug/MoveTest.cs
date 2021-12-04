using System;
using UnityEngine;

namespace CustomScripts.Gamemode.GMDebug
{
    public class MoveTest : MonoBehaviour
    {
        public bool isMoving = false;
        public float Speed;

        public void StartMoving()
        {
            isMoving = true;
        }

        private void Update()
        {
            if (isMoving)
            {
                transform.position += Vector3.forward * (Speed * Time.deltaTime);
                GameReferences.Instance.Player.transform.position += Vector3.forward * (Speed * Time.deltaTime);
            }
        }
    }
}