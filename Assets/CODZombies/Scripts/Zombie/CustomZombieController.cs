#if H3VR_IMPORTED
using System;
using System.Collections;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Objects.Window;
using CODZombies.Scripts.Player;
using FistVR;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CODZombies.Scripts.Zombie
{
    public enum State
    {
        Chase,
        AttackWindow,
        Dead
    }

    public enum SpeedType
    {
        Walk,
        FastWalk,
        Run
    }


    // TODO Call all animations from the code using crossfade instead of bad animator transitions
    public class CustomZombieController : ZombieController
    {
        public Action<float> OnZombieDied;
        public Action OnZombieInitialize;

        public GameObject HeadObject;

        [HideInInspector] public State State;
        [HideInInspector] public float Health;

        // Manages player's invincibility frames after getting hit by custom zombie
        private bool _hitThrottled;

        [HideInInspector] public Ragdoll Ragdoll;

        [HideInInspector] public Animator Animator;
        private RandomZombieSound _soundPlayer;
        private NavMeshAgent _agent;

        private const float agentUpdateInterval = .15f;
        private float _agentUpdateTimer;

        private SpeedType _moveSpeed;
        private int _animIndex;

        public ParticleSystem ExplosionPS;

        private void Awake()
        {
            State = State.Dead;

            _agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            _soundPlayer = GetComponent<RandomZombieSound>();
            Ragdoll = GetComponent<Ragdoll>();
        }

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            Animator.enabled = true;

            _agent.updateRotation = false;
            _agentUpdateTimer = agentUpdateInterval;

            _agent.speed = 0.1f;

            if (RoundManager.Instance.IsFastWalking)
            {
                int random = Random.Range(0, 3);

                _moveSpeed = SpeedType.FastWalk;
                _animIndex = random;

                if (GameSettings.HardMode)
                    Animator.SetFloat("FastWalkSpeed", 1.05f);
                else
                    Animator.SetFloat("FastWalkSpeed", .85f);
            }

            if (RoundManager.Instance.IsRunning)
            {
                int random = Random.Range(0, 4);

                _moveSpeed = SpeedType.Run;
                _animIndex = random;

                if (GameSettings.HardMode)
                    Animator.SetFloat("RunSpeed", Random.Range(.9f, .95f));
                else
                    Animator.SetFloat("RunSpeed", Random.Range(.8f, .85f));
            }

            if (!RoundManager.Instance.IsRunning && !RoundManager.Instance.IsFastWalking) // walking
            {
                int random = Random.Range(0, 4);

                _moveSpeed = SpeedType.Walk;
                _animIndex = random;

                if (GameSettings.HardMode)
                    Animator.SetFloat("WalkSpeed", 1.2f);
            }

            StartMovementAnimation();

            int currentRound = RoundManager.Instance.RoundNumber;

            Health = ZombieManager.Instance.CustomZombieHPCurve.Evaluate(currentRound);

            if (RoundManager.Instance.IsRoundSpecial)
                Health *= .65f;

            if (GameSettings.WeakerEnemiesEnabled)
                Health *= .65f;

            State = State.Chase;
            _agent.enabled = true;

            _soundPlayer.Initialize();

            if (OnZombieInitialize != null)
                OnZombieInitialize.Invoke();
        }

        public override void InitializeSpecialType()
        {
            Animator.SetFloat("WalkSpeed", 1.8f);
            Animator.SetFloat("FastWalkSpeed", 1.8f);
            Animator.SetFloat("RunSpeed", 1.8f);
        }

        private void Update()
        {
            if (State == State.Dead)
                return;

            if (State == State.AttackWindow)
                return;

            _agentUpdateTimer += Time.deltaTime;
            if (_agentUpdateTimer >= agentUpdateInterval)
            {
                _agentUpdateTimer -= agentUpdateInterval;
                _agent.SetDestination(Target.position);
            }

            if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(_agent.velocity.normalized), 10 * Time.deltaTime);
            }
        }


        // Theoretically they should be stunned when hitting window too, but little complicated and gives almost no benefit to the player
        public IEnumerator Stun()
        {
            if (State == State.Chase)
            {
                Animator.CrossFade("Stunned", .25f, 0, 0);

                yield return new WaitForSeconds(2f);

                StartMovementAnimation();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (State == State.Dead)
                return;

            if (other.GetComponent<ITrap>() != null)
            {
                other.GetComponent<ITrap>().OnEnemyEntered(this);
            }
            else if (other.GetComponent<WindowTrigger>() && !RoundManager.Instance.IsRoundSpecial)
            {
                Window window = other.GetComponent<WindowTrigger>().Window;
                if (State == State.AttackWindow)
                    return;
                if (window.IsOpen)
                {
                    ChangeTarget(GameReferences.Instance.Player);
                    return;
                }

                StartCoroutine(RotateTowards(window.transform.position, 5f));

                LastInteractedWindow = window;

                OnTouchingWindow();
            }
        }

        public override void OnHit(float damage, bool headHit = false)
        {
            if (Health <= 0 || State == State.Dead)
                return;

            float newDamage = damage * PlayerData.Instance.DamageModifier;

            damage = (int) newDamage;

            AudioManager.Instance.Play(AudioManager.Instance.ZombieHitSound, .7f);

            GameManager.Instance.AddPoints(ZombieManager.Instance.PointsOnHit);
            Health -= damage;

            if (Health <= 0 || PlayerData.Instance.InstaKill)
            {
                OnKill();
            }
            else // Hit animation
            {
                if (headHit && !_hitThrottled)
                {
                    Animator.SetTrigger("GetHit");
                    Animator.SetFloat("HitAngle", .5f);

                    StartCoroutine(HitAnimThrottle());
                }
            }
        }

        public override void OnKill(bool awardPoints = true)
        {
            _agent.speed = 0.1f;
            Animator.applyRootMotion = true;

            State = State.Dead;

            if (RoundManager.Instance.IsRoundSpecial)
            {
                Explode(awardPoints);
                return;
            }


            //////////
            // weird mumbo jumbo to minimize weird behavior that causes custom zombies
            // to jump upon death
            // int random = Random.Range(0, 3);
            // switch (random)
            // {
            //     case 0:
            //         if (OnZombieDied != null)
            //             OnZombieDied.Invoke(1.8f);
            //         break;
            //     case 1:
            //         if (OnZombieDied != null)
            //             OnZombieDied.Invoke(2.4f);
            //         break;
            //     case 2:
            //         if (OnZombieDied != null)
            //             OnZombieDied.Invoke(1.25f);
            //         break;
            // }
            //
            // _animator.CrossFade("Death" + random, 0.25f, 0, 0);
            //////////////////////////

            if (OnZombieDied != null)
                OnZombieDied.Invoke(0f);

            _agent.enabled = false;

            if (awardPoints)
            {
                GameManager.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);

                AudioManager.Instance.Play(AudioManager.Instance.ZombieDeathSound, .7f);
            }


            ZombieManager.Instance.OnZombieDied(this, awardPoints);
        }

        private void Explode(bool awardPoints)
        {
            if (OnZombieDied != null)
                OnZombieDied.Invoke(0f);
            _agent.enabled = false;

            if (awardPoints)
            {
                GameManager.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);
                AudioManager.Instance.Play(AudioManager.Instance.HellHoundDeathSound, .13f);
            }

            ZombieManager.Instance.OnZombieDied(this, awardPoints);

            var explosionPS = Instantiate(ZombieManager.Instance.HellhoundExplosionPS,
                transform.position + new Vector3(0, .75f, 0), transform.rotation);
            Destroy(explosionPS.gameObject, 4f);
        }

        private IEnumerator HitAnimThrottle()
        {
            _hitThrottled = true;
            yield return new WaitForSeconds(.25f);
            _hitThrottled = false;
        }

        public void ChangeTarget(Transform newTarget)
        {
            Target = newTarget;
        }

        public void OnHitPlayer()
        {
            if (State == State.Dead)
                return;

            if (PlayerData.Instance.IsInvincible)
                return;

            AudioManager.Instance.Play(AudioManager.Instance.PlayerHitSound);

            if (Application.isEditor)
                return;

            GM.CurrentPlayerBody.Health -= ZombieManager.Instance.CustomZombieDamage;

            if (PlayerData.GettingHitEvent != null)
                PlayerData.GettingHitEvent.Invoke();

            if (GM.CurrentPlayerBody.Health <= 0)
                GameManager.Instance.KillPlayer();
        }

        private IEnumerator RotateTowards(Vector3 target, float rotSpeed)
        {
            float timer = 0f;
            while (timer <= .5f)
            {
                yield return null;

                timer += Time.deltaTime;
                Vector3 dir = target - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, rotSpeed * Time.deltaTime, 0.0f);

                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }

        public void OnTouchingWindow()
        {
            if (State == State.Dead)
                return;

            _agent.speed = 0;

            Animator.CrossFade("Attack" + Random.Range(0, 4), 0.25f, 0, 0);
            State = State.AttackWindow;
        }

        // Called by animation
        public void OnHitWindow()
        {
            LastInteractedWindow.OnPlankRipped();
        }

        // Called by animation
        public void OnHitWindowEnd()
        {
            if (State == State.Dead)
                return;

            if (LastInteractedWindow.IsOpen)
            {
                State = State.Chase;
                _agent.speed = 0.1f;

                StartMovementAnimation();
                ChangeTarget(GameReferences.Instance.Player);
            }
            else
            {
                Animator.CrossFade("Attack" + Random.Range(0, 4), 0.25f, 0, 0);
            }
        }

        private void StartMovementAnimation()
        {
            string anim = "";
            switch (_moveSpeed)
            {
                case SpeedType.Walk:
                    anim = "Walk";
                    break;
                case SpeedType.FastWalk:
                    anim = "FastWalk";
                    break;
                case SpeedType.Run:
                    anim = "Run";
                    break;
            }

            anim += _animIndex.ToString();

            Animator.CrossFade(anim, 0.25f, 0, 0);
        }
    }
}
#endif