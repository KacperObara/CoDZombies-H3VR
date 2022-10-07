using FistVR;
using UnityEngine;

namespace CODZombies.Scripts
{
    public class MagazineWrapper : MonoBehaviour
    {
        private FVRFireArmMagazine _magazine;
        private FVRFireArmClip _clip;
        private Speedloader _speedloader;

        public FireArmRoundClass RoundClass;

        public void Initialize(FVRFireArmMagazine magazine)
        {
            _magazine = magazine;
            RoundClass = magazine.DefaultLoadingPattern.Classes[0];
        }

        public void InitialzieWithAmmo(FVRFireArmMagazine magazine, FireArmRoundClass roundClass)
        {
            _magazine = magazine;
            RoundClass = roundClass;
        }

        public void InitialzieWithAmmo(FVRFireArmClip clip, FireArmRoundClass roundClass)
        {
            _clip = clip;
            RoundClass = roundClass;
        }

        public void InitialzieWithAmmo(Speedloader speedloader, FireArmRoundClass roundClass)
        {
            _speedloader = speedloader;
            RoundClass = roundClass;
        }
    }
}