#if H3VR_IMPORTED

using CustomScripts.Managers;
using CustomScripts.Objects;
using CustomScripts.Player;
using CustomScripts.Powerups;
using UnityEngine;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                GameSettings.Instance.ToggleUseZosigs();
                RoundManager.Instance.StartGame();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (ZombieManager.Instance.ExistingZombies.Count > 0)
                    ZombieManager.Instance.ExistingZombies[0].OnHit(99999);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (ZombieManager.Instance.ExistingZombies.Count > 0)
                    ZombieManager.Instance.ExistingZombies[0].OnHit(1, true);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (ZombieManager.Instance.ExistingZombies.Count > 0)
                    ZombieManager.Instance.ExistingZombies[0].OnHit(2);
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


            if (Input.GetKeyDown(KeyCode.I))
            {
                TeleportToSecondArea.OnLeverPull();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                TeleportToMainArea.OnLeverPull();
            }


        }

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
    }
}

#endif