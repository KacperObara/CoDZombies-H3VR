#if H3VR_IMPORTED

using CustomScripts.Managers;
using CustomScripts.Objects;
using CustomScripts.Player;
using CustomScripts.Powerups;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts.Gamemode.GMDebug
{
    public class CustomDebug : MonoBehaviour
    {
        public Transform Point;

        public PowerUpCarpenter Carpenter;
        public PowerUpInstaKill InstaKill;
        public PowerUpDeathMachine DeathMachine;
        public PowerUpMaxAmmo MaxAmmo;
        public PowerUpDoublePoints DoublePoints;
        public PowerUpNuke Nuke;

        public Blockade TrapBlockade;
        public ElectroTrap ElectroTrap;

        public Teleport TeleportToSecondArea;
        public Teleport TeleportToMainArea;

        public DeadShotPerkBottle DeadShotPerkBottle;

        public LootPoolChoice newLootPool;

        ZombieBodyPart part;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                GameSettings.Instance.EnableCustomEnemiesClicked();
                RoundManager.Instance.StartGame();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (ZombieManager.Instance.ExistingZombies.Count > 0)
                {
                    Damage dam = new Damage();
                    dam.Class = Damage.DamageClass.Projectile;
                    dam.Dam_Piercing += 1 * 1f;
                    dam.Dam_TotalKinetic = 60000;
                    dam.hitNormal = Vector3.back;

                    ZombieManager.Instance.ExistingZombies[0].GetComponentInChildren<ZombieBodyPart>().Damage(dam);

                    part = ZombieManager.Instance.ExistingZombies[0]
                        .GetComponentInChildren<ZombieBodyPart>();
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Damage dam = new Damage();
                dam.Class = Damage.DamageClass.Projectile;
                dam.Dam_Piercing += 1 * 1f;
                dam.Dam_TotalKinetic = 60000;
                dam.hitNormal = Vector3.back;
                part.Damage(dam);
            }



            if (Input.GetKeyDown(KeyCode.D))
            {
                DeadShotPerkBottle.ApplyModifier();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PlayerData.Instance.ActivateStun());
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                GameManager.Instance.AddPoints(300);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                TrapBlockade.Buy();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                ElectroTrap.OnLeverPull();
            }

            if (Input.GetKeyDown(KeyCode.Slash))
            {
                GameManager.Instance.TurnOnPower();
            }


            if (Input.GetKeyDown(KeyCode.I))
            {
                TeleportToSecondArea.OnLeverPull();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                TeleportToMainArea.OnLeverPull();
            }





            if (Input.GetKeyDown(KeyCode.M))
            {
                GameSettings.Instance.ToggleBackgroundMusic();
            }


            // if (Input.GetKeyDown(KeyCode.LeftBracket))
            // {
            //     GameSettings.Instance.ChangeLootPool(newLootPool);
            // }

            // // if (Input.GetKeyDown(KeyCode.LeftBracket))
            // {
            //     GameSettings.Instance.DifficultyNormalClicked();
            // }
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                Choice1.ChangePoolToThis();
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                Choice2.ChangePoolToThis();
            }
        }

        public LootPoolChoice Choice1;
        public LootPoolChoice Choice2;

        public void SpawnCarpenter()
        {
            Carpenter.Spawn(Point.position);
        }

        public void SpawnInstaKill()
        {
            InstaKill.Spawn(Point.position);
        }

        public void SpawnDeathMachine()
        {
            DeathMachine.Spawn(Point.position);
        }

        public void SpawnMaxAmmo()
        {
            MaxAmmo.Spawn(Point.position);
        }

        public void SpawnDoublePoints()
        {
            DoublePoints.Spawn(Point.position);
        }

        public void SpawnNuke()
        {
            Nuke.Spawn(Point.position);
        }

        public void KillRandom()
        {
            if (ZombieManager.Instance.ExistingZombies.Count > 0)
                ZombieManager.Instance.ExistingZombies[0].OnHit(99999);
        }

        private bool _forcingSpecialEnemy;
        public void ToggleForceSpecialRound()
        {
            if (_forcingSpecialEnemy)
            {
                RoundManager.Instance.SpecialRoundInterval = 8;
                _forcingSpecialEnemy = false;
            }
            else
            {
                RoundManager.Instance.SpecialRoundInterval = 1;
                _forcingSpecialEnemy = true;
            }
        }
    }
}

#endif