#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.Serialization;
namespace CustomScripts.Gamemode.GMDebug
{
    public class MoveTest : MonoBehaviour
    {
        [FormerlySerializedAs("isMoving")] public bool IsMoving;
        public float Speed;

        private void Update()
        {
            if (IsMoving)
            {
                transform.position += Vector3.forward * (Speed * Time.deltaTime);
                GameReferences.Instance.Player.transform.position += Vector3.forward * (Speed * Time.deltaTime);
            }
        }

        public void StartMoving()
        {
            IsMoving = true;
        }
    }
}
#endif