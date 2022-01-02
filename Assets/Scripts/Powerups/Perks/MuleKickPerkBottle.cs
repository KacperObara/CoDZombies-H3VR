#if H3VR_IMPORTED
using Atlas.MappingComponents.Sandbox;
using FistVR;
using UnityEngine;
namespace CustomScripts.Powerups.Perks
{
    public class MuleKickPerkBottle : MonoBehaviour, IModifier
    {
        public ObjectSpawnPoint Spawner;
        public string ObjectID;

        public GameObject Model;

        public void ApplyModifier()
        {
            Spawner.ObjectId = ObjectID;
            Spawner.Spawn();

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