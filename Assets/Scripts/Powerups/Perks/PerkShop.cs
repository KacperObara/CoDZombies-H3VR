using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class PerkShop : MonoBehaviour
    {
        public int Cost;

        public GameObject Bottle;
        public Transform SpawnPoint;

        private bool alreadyUsed = false;

        public void TryBuying()
        {
            if (alreadyUsed)
                return;

            if (GameManager.Instance.TryRemovePoints(Cost))
            {
                alreadyUsed = true;

                Bottle.transform.position = SpawnPoint.position;

                AudioManager.Instance.BuySound.Play();
            }
        }
    }
}