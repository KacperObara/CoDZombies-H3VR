#if H3VR_IMPORTED
using CustomScripts.Player;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class ZombieBodyPart : MonoBehaviour, IFVRDamageable
    {
        public int PartDamageMultiplier = 1;
        public CustomZombieController Controller;

        public void Damage(Damage dam)
        {
            if (dam.Class == FistVR.Damage.DamageClass.Melee)
            {
                Controller.OnHit(1);
                return;
            }

            // TODO may need to rethink explosives, for example grenades hit many times many body parts (I think)
            if (dam.Class == FistVR.Damage.DamageClass.Explosive)
            {
                Controller.OnHit(1);
                return;
            }

            if (dam.Dam_TotalKinetic < 20)
                return;

            int damage = 0;

            if (dam.Dam_TotalKinetic < 1000)
                damage = 1;
            else if (dam.Dam_TotalKinetic < 2000)
                damage = 2;
            else if (dam.Dam_TotalKinetic >= 2000)
                damage = 3;

            if (dam.Dam_TotalKinetic > 10000)
                damage = 20;


            damage *= PartDamageMultiplier;

            if (GameSettings.LimitedAmmo)
                damage *= 2;


            if (PartDamageMultiplier == 3)
            {
                // Custom zombies operate on small damage numbers for now, so this modifier can change damage by less or more than 20%
                if (PlayerData.Instance.DeadShotPerkActivated)
                    damage = Mathf.CeilToInt(damage * 1.2f);

                Controller.OnHit(damage, true);
            }
            else
            {
                Controller.OnHit(damage);
            }
        }
    }
}
#endif