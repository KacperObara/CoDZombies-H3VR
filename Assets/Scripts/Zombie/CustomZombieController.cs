#if H3VR_IMPORTED
using System;
using System.Collections;
using CustomScripts.Managers;
using CustomScripts.Player;
using FistVR;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts.Zombie
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
        public static int PlayerTouchCount;
        public static bool IsBeingHit;
        private bool _hitThrottled;

        private Animator _animator;
        private RandomZombieSound _soundPlayer;
        private NavMeshAgent _agent;

        private const float agentUpdateInterval = .15f;
        private float _agentUpdateTimer;

        private SpeedType _moveSpeed;
        private int _animIndex;

        private void Awake()
        {
            State = State.Dead;
        }

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _soundPlayer = GetComponent<RandomZombieSound>();
        }

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            _agent.updateRotation = false;
            _agentUpdateTimer = agentUpdateInterval;

            _agent.speed = 0.1f;

            if (RoundManager.Instance.IsFastWalking)
            {
                int random = Random.Range(0, 3);

                _moveSpeed = SpeedType.FastWalk;
                _animIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("FastWalkSpeed", 1.05f);
                else
                    _animator.SetFloat("FastWalkSpeed", .85f);
            }

            if (RoundManager.Instance.IsRunning)
            {
                int random = Random.Range(0, 4);

                _moveSpeed = SpeedType.Run;
                _animIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("RunSpeed", Random.Range(.9f, .95f));
                else
                    _animator.SetFloat("RunSpeed", Random.Range(.8f, .85f));
            }

            if (!RoundManager.Instance.IsRunning && !RoundManager.Instance.IsFastWalking) // walking
            {
                int random = Random.Range(0, 4);

                _moveSpeed = SpeedType.Walk;
                _animIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("WalkSpeed", 1.2f);
            }

            StartMovementAnimation();

            int currentRound = RoundManager.Instance.RoundNumber;

            if (GameSettings.WeakerEnemies)
                Health = ZombieManager.Instance.CustomZombieHPCurve.Evaluate(currentRound - 5);
            else
                Health = ZombieManager.Instance.CustomZombieHPCurve.Evaluate(currentRound);

            State = State.Chase;
            _agent.enabled = true;

            _soundPlayer.Initialize();

            if (OnZombieInitialize != null)
                OnZombieInitialize.Invoke();
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
                _animator.CrossFade("Stunned", .25f, 0, 0);

                yield return new WaitForSeconds(2f);

                StartMovementAnimation();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (State == State.Dead)
                return;

            if (other.GetComponent<WindowTrigger>())
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

        // private void OnTriggerExit(Collider other)
        // {
        //     if (State == State.Dead)
        //         return;
        //
        //     if (other.GetComponent<WindowTrigger>())
        //     {
        //         if (State == State.AttackWindow)
        //         {
        //             State = State.Chase;
        //             _agent.speed = 0.1f;
        //
        //             StartMovementAnimation();
        //             ChangeTarget(GameReferences.Instance.Player);
        //         }
        //     }
        // }

        public override void OnHit(float damage, bool headHit = false)
        {
            if (Health <= 0 || State == State.Dead)
                return;

            float newDamage = damage * PlayerData.Instance.DamageModifier;

            damage = (int) newDamage;

            AudioManager.Instance.ZombieHitSound.Play();
            GameManager.Instance.AddPoints(ZombieManager.Instance.PointsOnHit);
            Health -= damage;

            if (Health <= 0 || PlayerData.Instance.InstaKill)
            {
                _agent.speed = 0.1f;
                _animator.applyRootMotion = true;

                State = State.Dead;

                // weird mumbo jumbo to minimize weird behavior that causes custom zombies
                // to jump upon death
                int random = Random.Range(0, 3);
                switch (random)
                {
                    case 0:
                        if (OnZombieDied != null)
                            OnZombieDied.Invoke(1.8f);
                        break;
                    case 1:
                        if (OnZombieDied != null)
                            OnZombieDied.Invoke(2.4f);
                        break;
                    case 2:
                        if (OnZombieDied != null)
                            OnZombieDied.Invoke(1.25f);
                        break;
                }

                _animator.CrossFade("Death" + random, 0.25f, 0, 0);

                _agent.enabled = false;
                GameManager.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);

                AudioManager.Instance.ZombieDeathSound.Play();

                ZombieManager.Instance.OnZombieDied(this);
            }
            else // Hit animation
            {
                if (headHit && !_hitThrottled)
                {
                    _animator.SetTrigger("GetHit");
                    _animator.SetFloat("HitAngle", .5f);

                    StartCoroutine(HitAnimThrottle());
                }
            }
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

            IsBeingHit = true;
            PlayerTouchCount++;

            AudioManager.Instance.PlayerHitSound.Play();
            GM.CurrentPlayerBody.Health -= ZombieManager.Instance.CustomZombieDamage;

            if (PlayerData.GettingHitEvent != null)
                PlayerData.GettingHitEvent.Invoke();

            StartCoroutine(CheckStillColliding());

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
            _agent.speed = 0;

            _animator.CrossFade("Attack" + Random.Range(0, 4), 0.25f, 0, 0);
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
            if (LastInteractedWindow.IsOpen)
            {
                State = State.Chase;
                _agent.speed = 0.1f;

                StartMovementAnimation();
                ChangeTarget(GameReferences.Instance.Player);
            }
            else
            {
                _animator.CrossFade("Attack" + Random.Range(0, 4), 0.25f, 0, 0);
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

            _animator.CrossFade(anim, 0.25f, 0, 0);
        }

        public void OnPlayerTouch()
        {
            if (PlayerTouchCount != 0)
                return;

            if (IsBeingHit)
                return;

            OnHitPlayer();
        }

        public void OnPlayerStopTouch()
        {
            if (PlayerTouchCount == 0)
                return;

            PlayerTouchCount--;
        }

        private IEnumerator CheckStillColliding()
        {
            yield return new WaitForSeconds(1.5f);

            if (PlayerTouchCount != 0 && !GameManager.Instance.GameEnded)
            {
                OnHitPlayer();
            }

            IsBeingHit = false;
        }
    }
}
#endif