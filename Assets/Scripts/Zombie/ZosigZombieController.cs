using System;
using System.Collections;
using CustomScripts.Managers;
using CustomScripts.Player;
using UnityEngine;
using UnityEngine.AI;
using Damage = FistVR.Damage;
using Sosig = FistVR.Sosig;
using SosigLink = FistVR.SosigLink;

namespace CustomScripts.Zombie
{
    public class ZosigZombieController : ZombieController
    {
        private float agentUpdateTimer;
        private const float agentUpdateInterval = 1f;

        private Sosig sosig;

        private int hitsGivingMoney = 6;

        private bool isDead = false;
        private bool isAttackingWindow = false;

        private float cachedSpeed;

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            sosig = GetComponent<Sosig>();

            sosig.CoreRB.gameObject.AddComponent<ZosigTrigger>().Initialize(this);

            sosig.Speed_Run = 2f;
            if (RoundManager.Instance.IsFastWalking)
            {
                sosig.Speed_Run = 3f;
            }

            if (RoundManager.Instance.IsRunning)
            {
                sosig.Speed_Run = 4f;
            }

            if (GameSettings.FasterEnemies)
            {
                sosig.Speed_Run += 1.5f;
            }


            sosig.Mustard = 100 + (20 * RoundManager.Instance.RoundNumber);

            foreach (var link in sosig.Links)
            {
                link.SetIntegrity(100 + (15 * RoundManager.Instance.RoundNumber));
            }

            if (GameSettings.WeakerEnemies)
            {
                sosig.Mustard = 70 + (15 * RoundManager.Instance.RoundNumber);

                foreach (var link in sosig.Links)
                {
                    link.SetIntegrity(80 + (10 * RoundManager.Instance.RoundNumber));
                }
            }

            sosig.DamMult_Melee = 0;

            sosig.Speed_Walk = sosig.Speed_Run;
            sosig.Speed_Turning = sosig.Speed_Run;
            sosig.Speed_Sneak = sosig.Speed_Run;

            //sosig.GetHeldMeleeWeapon().O.IsPickUpLocked = true;
            CheckPerks();
        }

        public void CheckPerks()
        {
            if (PlayerData.Instance.DeadShotPerkActivated)
            {
                //sosig.DamMult_Projectile = 1.25f;
                sosig.Links[0].DamMult = 1.15f;
            }

            if (PlayerData.Instance.DoubleTapPerkActivated)
            {
                sosig.DamMult_Projectile = 1.25f;
            }
        }

        private void Update()
        {
            if (sosig == null)
                return;

            agentUpdateTimer += Time.deltaTime;
            if (agentUpdateTimer >= agentUpdateInterval)
            {
                agentUpdateTimer -= agentUpdateInterval;

                sosig.FallbackOrder = Sosig.SosigOrder.Assault;
                //sosig.BrainUpdate_Assault();
                sosig.UpdateGuardPoint(Target.position);
                sosig.UpdateAssaultPoint(Target.position);

                // Quick hack if sosigs try to follow you but on the wrong floor
                if (sosig.Agent.destination.y + 3f < Target.position.y)
                {
                    sosig.UpdateAssaultPoint(Target.position + Vector3.up);
                }

                if (sosig.Agent.destination.y > Target.position.y + 3f)
                {
                    sosig.UpdateAssaultPoint(Target.position + Vector3.down);
                }

                sosig.SetCurrentOrder(Sosig.SosigOrder.Assault);
            }
        }

        public void OnKill()
        {
            if (!ZombieManager.Instance.ExistingZombies.Contains(this))
                return;

            isDead = true;

            GameManager.Instance.AddPoints(100);

            ZombieManager.Instance.OnZombieDied(this);

            StartCoroutine(DelayedDespawn());
        }

        public void OnGetHit(Damage damage)
        {
            if (damage.Dam_TotalKinetic < 20)
                return;

            if (PlayerData.Instance.InstaKill)
            {
                sosig.KillSosig();
            }

            if (hitsGivingMoney <= 0)
                return;

            hitsGivingMoney--;

            GameManager.Instance.AddPoints(10);
        }

        public override void OnHit(float damage, bool headHit)
        {
            //nuke
            sosig.Links[0].LinkExplodes(Damage.DamageClass.Projectile);
            sosig.KillSosig();
        }

        public override void OnHitPlayer()
        {
        }

        public override void ChangeTarget(Transform newTarget)
        {
            Target = newTarget;
        }

        public void OnTriggerEntered(Collider other)
        {
            if (isDead)
                return;

            if (isAttackingWindow)
                return;

            if (other.GetComponent<WindowTrigger>())
            {
                Window window = other.GetComponent<WindowTrigger>().Window;
                if (window.IsOpen)
                {
                    ChangeTarget(GameReferences.Instance.Player);
                    return;
                }

                isAttackingWindow = true;

                cachedSpeed = sosig.Speed_Run;
                sosig.Speed_Run = 0;

                LastInteractedWindow = window;
                OnTouchingWindow();
            }
        }

        public void OnTouchingWindow() // refactor this
        {
            StartCoroutine(TearPlankDelayed());
        }

        public void OnHitWindow()
        {
            LastInteractedWindow.OnPlankRipped();

            if (LastInteractedWindow.IsOpen)
            {
                ChangeTarget(GameReferences.Instance.Player);
            }
        }

        private IEnumerator TearPlankDelayed()
        {
            while (!LastInteractedWindow.IsOpen && !isDead)
            {
                yield return new WaitForSeconds(2.5f);

                if (!isDead && sosig.BodyState == Sosig.SosigBodyState.InControl)
                    OnHitWindow();
            }

            isAttackingWindow = false;
            sosig.Speed_Run = cachedSpeed;
        }

        private IEnumerator DelayedDespawn()
        {
            yield return new WaitForSeconds(5);
            sosig.DeSpawnSosig();
        }
    }
}