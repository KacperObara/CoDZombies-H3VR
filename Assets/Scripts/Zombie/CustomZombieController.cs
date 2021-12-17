using System;
using System.Collections;
using CustomScripts.Managers;
using CustomScripts.Player;
using FistVR;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
namespace CustomScripts.Zombie
{
    public enum State
    {
        Chase,
        AttackWindow,
        Dead
    }

    // TODO string based property lookup is inefficient, also awful code readability
    public class CustomZombieController : ZombieController
    {
        private const float AGENT_UPDATE_INTERVAL = 1f;

        public static int Playertouches;
        public static bool IsBeingHit;

        public int PointsOnHit = 10;
        public int PointsOnKill = 100;

        public GameObject HeadObject;

        [HideInInspector] public State State;
        [HideInInspector] public float Health;

        public int RunAnimationIndex;
        public int AnimIndex;
        private NavMeshAgent _agent;

        private float _agentUpdateTimer;

        private Animator _animator;

        private bool _hitThrottled;
        private RandomZombieSound _soundPlayer;
        public Action<float> OnZombieDied;
        public Action OnZombieInitialize;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _soundPlayer = GetComponent<RandomZombieSound>();
        }

        private void Update()
        {
            if (State == State.Dead)
                return;

            if (State == State.AttackWindow)
                return;

            _agentUpdateTimer += Time.deltaTime;
            if (_agentUpdateTimer >= AGENT_UPDATE_INTERVAL)
            {
                _agentUpdateTimer -= AGENT_UPDATE_INTERVAL;
                _agent.SetDestination(Target.position);
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

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            _agentUpdateTimer = AGENT_UPDATE_INTERVAL;
            _animator.SetBool("IsDead", false);
            _animator.SetBool("IsAttacking", false);
            _animator.SetBool("IsIdle", false);
            _animator.SetBool("IsMoving", true);

            _agent.speed = 0.1f;

            if (RoundManager.Instance.IsFastWalking)
            {
                int random = Random.Range(0, 3);
                _animator.SetInteger("RunAnimationIndex", 1);
                _animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 1;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("FastWalkSpeed", 1.2f);
            }

            if (RoundManager.Instance.IsRunning)
            {
                int random = Random.Range(0, 4);
                _animator.SetInteger("RunAnimationIndex", 2);
                _animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 2;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("RunSpeed", Random.Range(.75f, .85f));
                else
                    _animator.SetFloat("RunSpeed", Random.Range(.9f, 1f));
            }

            if (!RoundManager.Instance.IsRunning && !RoundManager.Instance.IsFastWalking) // walking
            {
                int random = Random.Range(0, 4);
                _animator.SetInteger("RunAnimationIndex", 0);
                _animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 0;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    _animator.SetFloat("WalkSpeed", 1.2f);
            }

            if (GameSettings.WeakerEnemies)
                Health = RoundManager.Instance.WeakerZombieHp;
            else
                Health = RoundManager.Instance.ZombieHp;

            State = State.Chase;
            _agent.enabled = true;

            _soundPlayer.Initialize();
            OnZombieInitialize.Invoke();
        }

        public override void OnHit(float damage, bool headHit = false)
        {
            if (Health <= 0 || State == State.Dead)
                return;

            if (!ZombieManager.Instance.ExistingZombies.Contains(this))
            {
                Debug.LogWarning("Trying to kill a zombie that should not exist!");
                Debug.LogWarning("Health: " + Health + " Damage " + damage);
                return;
            }

            float newDamage = damage * PlayerData.Instance.DamageModifier;

            damage = (int)newDamage;

            AudioManager.Instance.ZombieHitSound.Play();
            GameManager.Instance.AddPoints(PointsOnHit);
            Health -= damage;

            if (Health <= 0 || PlayerData.Instance.InstaKill)
            {
                _agent.speed = 0.1f;
                _animator.applyRootMotion = true;

                State = State.Dead;

                int random = Random.Range(0, 3);
                switch (random)
                {
                    case 0:
                        OnZombieDied.Invoke(1.8f);
                        break;
                    case 1:
                        OnZombieDied.Invoke(2.4f);
                        break;
                    case 2:
                        OnZombieDied.Invoke(1.25f);
                        break;
                }

                _animator.SetInteger("AnimIndex", random);

                _animator.SetBool("IsDead", true);
                _animator.SetBool("IsMoving", false);
                _animator.SetBool("IsAttacking", false);
                _animator.SetBool("IsIdle", false);

                _agent.enabled = false;
                GameManager.Instance.AddPoints(PointsOnKill);

                AudioManager.Instance.ZombieDeathSound.Play();

                ZombieManager.Instance.OnZombieDied(this);
            }
            else // Hit animation
            {
                if (headHit && !_hitThrottled)
                {
                    _animator.SetTrigger("GetHit");
                    //animator.SetFloat("HitAngle", Random.Range(0f, 1f));
                    _animator.SetFloat("HitAngle", .5f);

                    StartCoroutine(HitAnimThrottle());
                    //animator.SetLayerWeight(1, 1f);
                }
            }
        }

        private IEnumerator HitAnimThrottle()
        {
            _hitThrottled = true;
            yield return new WaitForSeconds(.25f);
            _hitThrottled = false;
        }

        public override void ChangeTarget(Transform newTarget)
        {
            Target = newTarget;
        }

        public override void OnHitPlayer()
        {
            if (State == State.Dead)
                return;

            IsBeingHit = true;
            Playertouches++;

            AudioManager.Instance.PlayerHitSound.Play();
            GM.CurrentPlayerBody.Health -= 2000;
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
            //animator.applyRootMotion = false;
            _animator.SetBool("IsAttacking", true);
            _animator.SetBool("IsMoving", false);
            _animator.SetInteger("AnimIndex", Random.Range(0, 4));
            State = State.AttackWindow;
        }

        public void OnHitWindow()
        {
            LastInteractedWindow.OnPlankRipped();
            _animator.SetInteger("AnimIndex", Random.Range(0, 4));
        }

        public void OnHitWindowEnd()
        {
            if (LastInteractedWindow.IsOpen)
            {
                _animator.SetBool("IsAttacking", false);
                _animator.SetBool("IsMoving", true);
                _animator.SetInteger("RunAnimationIndex", RunAnimationIndex);
                _animator.SetInteger("AnimIndex", AnimIndex);

                State = State.Chase;
                _agent.speed = 0.1f;
                //animator.applyRootMotion = true;

                ChangeTarget(GameReferences.Instance.Player);
            }
        }

        public void OnPlayerTouch()
        {
            if (Playertouches != 0)
                return;

            if (IsBeingHit)
                return;

            OnHitPlayer();
        }

        public void OnPlayerStopTouch()
        {
            if (Playertouches == 0)
                return;

            Playertouches--;
        }

        private IEnumerator CheckStillColliding()
        {
            yield return new WaitForSeconds(1.5f);

            if (Playertouches != 0 && !GameManager.Instance.GameEnded)
            {
                OnHitPlayer();
            }

            IsBeingHit = false;
        }
    }
}