using FistVR;
using UnityEngine;

namespace CustomScripts.Gamemode
{
    public class MagazineWrapper : MonoBehaviour
    {
        private FVRFireArmMagazine _magazine;

        public FireArmRoundClass RoundClass;

        public void Initialize(FVRFireArmMagazine magazine)
        {
            _magazine = magazine;
        }

        // public void OnMagazineRetrievedFromBox()
        // {
        //
        // }
    }
}