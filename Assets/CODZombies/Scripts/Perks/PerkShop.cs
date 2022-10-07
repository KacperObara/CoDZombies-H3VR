using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
    public class PerkShop : MonoBehaviour, IPurchasable, IRequiresPower
    {
        public AudioSource PurchaseJingle;

        public int Cost;
        public int PurchaseCost { get { return Cost; } }
        [SerializeField] private bool _isOneTimeOnly = true;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }
        public bool IsPowered { get { return GameManager.Instance.PowerEnabled; } }

        public GameObject Bottle;
        public Transform SpawnPoint;

        private bool alreadyUsed = false;

        public void TryBuying()
        {
            if (alreadyUsed)
                return;

            if (IsPowered && GameManager.Instance.TryRemovePoints(Cost))
            {
                alreadyUsed = true;

                Bottle.transform.position = SpawnPoint.position;

                PurchaseJingle.Play();
            }
        }
    }
}