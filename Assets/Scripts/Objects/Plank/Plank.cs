#if H3VR_IMPORTED

using System.Collections;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class Plank : MonoBehaviour
    {
        [HideInInspector] public FVRPhysicalObject PhysicalObject;
        public Transform RestTransform;

        private void Awake()
        {
            PhysicalObject = GetComponent<FVRPhysicalObject>();
        }

        public void ReturnToRest()
        {
            StartCoroutine(MoveTo(RestTransform, .5f));
        }

        public void OnRepairDrop(Transform destination)
        {
            StartCoroutine(MoveTo(destination, .2f));

            AudioManager.Instance.Play(AudioManager.Instance.BarricadeRepairSound, .5f);
        }


        public IEnumerator MoveTo(Transform destination, float time)
        {
            Vector3 startingPos = transform.position;
            Vector3 finalPos = destination.position;
            Quaternion startingRot = transform.rotation;
            Quaternion finalRot = destination.rotation;

            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
                transform.rotation = Quaternion.Lerp(startingRot, finalRot, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = finalPos;
            transform.rotation = finalRot;
        }
    }
}
#endif