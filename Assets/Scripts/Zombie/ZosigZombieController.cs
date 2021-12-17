using System.Collections;
using CustomScripts.Managers;
using CustomScripts.Player;
using FistVR;
using UnityEngine;
namespace CustomScripts.Zombie
{
    public class ZosigZombieController : ZombieController
    {
        private const float AGENT_UPDATE_INTERVAL = 1f;
        private float _agentUpdateTimer;

        private float _cachedSpeed;

        private int _hitsGivingMoney = 6;
        private bool _isAttackingWindow;

        private bool _isDead;

        private Sosig _sosig;

        private void Update()
        {
            if (_sosig == null)
                return;

            _agentUpdateTimer += Time.deltaTime;
            if (_agentUpdateTimer >= AGENT_UPDATE_INTERVAL)
            {
                _agentUpdateTimer -= AGENT_UPDATE_INTERVAL;

                _sosig.FallbackOrder = Sosig.SosigOrder.Assault;
                //sosig.BrainUpdate_Assault();
                _sosig.UpdateGuardPoint(Target.position);
                _sosig.UpdateAssaultPoint(Target.position);

                // Quick hack if sosigs try to follow you but on the wrong floor
                if (_sosig.Agent.destination.y + 3f < Target.position.y)
                {
                    _sosig.UpdateAssaultPoint(Target.position + Vector3.up);
                }

                if (_sosig.Agent.destination.y > Target.position.y + 3f)
                {
                    _sosig.UpdateAssaultPoint(Target.position + Vector3.down);
                }

                _sosig.SetCurrentOrder(Sosig.SosigOrder.Assault);
            }
        }

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            _sosig = GetComponent<Sosig>();

            _sosig.CoreRB.gameObject.AddComponent<ZosigTrigger>().Initialize(this);

            _sosig.Speed_Run = 2f;
            if (RoundManager.Instance.IsFastWalking)
            {
                _sosig.Speed_Run = 3f;
            }

            if (RoundManager.Instance.IsRunning)
            {
                _sosig.Speed_Run = 4f;
            }

            if (GameSettings.FasterEnemies)
            {
                _sosig.Speed_Run += 1.5f;
            }


            _sosig.Mustard = 100 + 20 * RoundManager.Instance.RoundNumber;

            foreach (SosigLink link in _sosig.Links)
            {
                link.SetIntegrity(100 + (15 * RoundManager.Instance.RoundNumber));
            }

            if (GameSettings.WeakerEnemies)
            {
                _sosig.Mustard = 70 + 15 * RoundManager.Instance.RoundNumber;

                foreach (SosigLink link in _sosig.Links)
                {
                    link.SetIntegrity(80 + (10 * RoundManager.Instance.RoundNumber));
                }
            }

            _sosig.DamMult_Melee = 0;

            _sosig.Speed_Walk = _sosig.Speed_Run;
            _sosig.Speed_Turning = _sosig.Speed_Run;
            _sosig.Speed_Sneak = _sosig.Speed_Run;

            //sosig.GetHeldMeleeWeapon().O.IsPickUpLocked = true;
            CheckPerks();
        }

        public void CheckPerks()
        {
            if (PlayerData.Instance.DeadShotPerkActivated)
            {
                //sosig.DamMult_Projectile = 1.25f;
                _sosig.Links[0].DamMult = 1.15f;
            }

            if (PlayerData.Instance.DoubleTapPerkActivated)
            {
                _sosig.DamMult_Projectile = 1.25f;
            }
        }

        public void OnKill()
        {
            if (!ZombieManager.Instance.ExistingZombies.Contains(this))
                return;

            _isDead = true;

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
                _sosig.KillSosig();
            }

            if (_hitsGivingMoney <= 0)
                return;

            _hitsGivingMoney--;

            GameManager.Instance.AddPoints(10);
        }

        public override void OnHit(float damage, bool headHit)
        {
            //nuke
            _sosig.Links[0].LinkExplodes(Damage.DamageClass.Projectile);
            _sosig.KillSosig();
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
            if (_isDead)
                return;

            if (_isAttackingWindow)
                return;

            if (other.GetComponent<WindowTrigger>())
            {
                Window window = other.GetComponent<WindowTrigger>().Window;
                if (window.IsOpen)
                {
                    ChangeTarget(GameReferences.Instance.Player);
                    return;
                }

                _isAttackingWindow = true;

                _cachedSpeed = _sosig.Speed_Run;
                _sosig.Speed_Run = 0;

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
            while (!LastInteractedWindow.IsOpen && !_isDead)
            {
                yield return new WaitForSeconds(2.5f);

                if (!_isDead && _sosig.BodyState == Sosig.SosigBodyState.InControl)
                    OnHitWindow();
            }

            _isAttackingWindow = false;
            _sosig.Speed_Run = _cachedSpeed;
        }

        private IEnumerator DelayedDespawn()
        {
            yield return new WaitForSeconds(5);
            _sosig.DeSpawnSosig();
        }
    }
}