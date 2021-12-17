#if H3VR_IMPORTED
using UnityEngine;
namespace CustomScripts
{
    public class PerkShop : MonoBehaviour
    {
        public int Cost;

        public GameObject Bottle;
        public Transform SpawnPoint;

        private bool _alreadyUsed;

        public void TryBuying()
        {
            if (_alreadyUsed)
                return;

            if (GameManager.Instance.TryRemovePoints(Cost))
            {
                _alreadyUsed = true;

                Bottle.transform.position = SpawnPoint.position;

                AudioManager.Instance.BuySound.Play();
            }
        }
    }
}
#endif