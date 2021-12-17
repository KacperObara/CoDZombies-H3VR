#if H3VR_IMPORTED
using FistVR;
using UnityEngine;
namespace CustomScripts.Powerups.Perks
{
    public class MuleKickPerkBottle : MonoBehaviour, IModifier
    {
        public ItemSpawner Spawner;
        public string ObjectID;

        public GameObject Model;

        public void ApplyModifier()
        {
            //TODO: New itemspawner doesnt have ObjectId field
            // Spawner.ObjectId = ObjectID;
            Spawner.SpawnItem();

            AudioManager.Instance.DrinkSound.Play();

            GetComponent<FVRPhysicalObject>().IsPickUpLocked = true;
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            GetComponent<Collider>().enabled = false;
            Model.SetActive(false);

            Destroy(gameObject, 1f);
        }
    }
}
#endif