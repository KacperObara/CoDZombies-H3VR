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
        public Action OnZombieInitialize;
        public Action<float> OnZombieDied;

        public int PointsOnHit = 10;
        public int PointsOnKill = 100;

        public GameObject HeadObject;

        [HideInInspector] public State State;
        [HideInInspector] public float Health;

        private Animator animator;
        private NavMeshAgent agent;
        private RandomZombieSound soundPlayer;

        private float agentUpdateTimer;
        private const float agentUpdateInterval = 1f;

        public int RunAnimationIndex;
        public int AnimIndex;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            soundPlayer = GetComponent<RandomZombieSound>();
        }

        public override void Initialize(Transform newTarget)
        {
            Target = newTarget;

            agentUpdateTimer = agentUpdateInterval;
            animator.SetBool("IsDead", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsMoving", true);

            agent.speed = 0.1f;

            if (RoundManager.Instance.IsFastWalking)
            {
                int random = Random.Range(0, 3);
                animator.SetInteger("RunAnimationIndex", 1);
                animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 1;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    animator.SetFloat("FastWalkSpeed", 1.2f);
            }

            if (RoundManager.Instance.IsRunning)
            {
                int random = Random.Range(0, 4);
                animator.SetInteger("RunAnimationIndex", 2);
                animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 2;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    animator.SetFloat("RunSpeed", Random.Range(.75f, .85f));
                else
                    animator.SetFloat("RunSpeed", Random.Range(.9f, 1f));
            }

            if (!RoundManager.Instance.IsRunning && !RoundManager.Instance.IsFastWalking) // walking
            {
                int random = Random.Range(0, 4);
                animator.SetInteger("RunAnimationIndex", 0);
                animator.SetInteger("AnimIndex", random);
                RunAnimationIndex = 0;
                AnimIndex = random;

                if (GameSettings.FasterEnemies)
                    animator.SetFloat("WalkSpeed", 1.2f);
            }

            if (GameSettings.WeakerEnemies)
                Health = RoundManager.Instance.WeakerZombieHP;
            else
                Health = RoundManager.Instance.ZombieHP;

            State = State.Chase;
            agent.enabled = true;

            soundPlayer.Initialize();
            OnZombieInitialize?.Invoke();
        }

        private void Update()
        {
            if (State == State.Dead)
                return;

            if (State == State.AttackWindow)
                return;

            agentUpdateTimer += Time.deltaTime;
            if (agentUpdateTimer >= agentUpdateInterval)
            {
                agentUpdateTimer -= agentUpdateInterval;
                agent.SetDestination(Target.position);
            }
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

            damage = (int) newDamage;

            AudioManager.Instance.ZombieHitSound.Play();
            GameManager.Instance.AddPoints(PointsOnHit);
            Health -= damage;

            if (Health <= 0 || PlayerData.Instance.InstaKill)
            {
                agent.speed = 0.1f;
                animator.applyRootMotion = true;

                State = State.Dead;

                int random = Random.Range(0, 3);
                switch (random)
                {
                    case 0:
                        OnZombieDied?.Invoke(1.8f);
                        break;
                    case 1:
                        OnZombieDied?.Invoke(2.4f);
                        break;
                    case 2:
                        OnZombieDied?.Invoke(1.25f);
                        break;
                }

                animator.SetInteger("AnimIndex", random);

                animator.SetBool("IsDead", true);
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsIdle", false);

                agent.enabled = false;
                GameManager.Instance.AddPoints(PointsOnKill);

                AudioManager.Instance.ZombieDeathSound.Play();

                ZombieManager.Instance.OnZombieDied(this);
            }
            else // Hit animation
            {
                if (headHit && !hitThrottled)
                {
                    animator.SetTrigger("GetHit");
                    //animator.SetFloat("HitAngle", Random.Range(0f, 1f));
                    animator.SetFloat("HitAngle", .5f);

                    StartCoroutine(HitAnimThrottle());
                    //animator.SetLayerWeight(1, 1f);
                }
            }
        }

        private bool hitThrottled = false;

        private IEnumerator HitAnimThrottle()
        {
            hitThrottled = true;
            yield return new WaitForSeconds(.25f);
            hitThrottled = false;
        }

        public override void ChangeTarget(Transform newTarget)
        {
            Target = newTarget;
        }

        public override void OnHitPlayer()
        {
            if (State == State.Dead)
                return;

            isBeingHit = true;
            playertouches++;

            AudioManager.Instance.PlayerHitSound.Play();
            GM.CurrentPlayerBody.Health -= 2000;
            StartCoroutine(CheckStillColliding());

            if (GM.CurrentPlayerBody.Health <= 0)
                GameManager.Instance.KillPlayer();
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
            agent.speed = 0;
            //animator.applyRootMotion = false;
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsMoving", false);
            animator.SetInteger("AnimIndex", Random.Range(0, 4));
            State = State.AttackWindow;
        }

        public void OnHitWindow()
        {
            LastInteractedWindow.OnPlankRipped();
            animator.SetInteger("AnimIndex", Random.Range(0, 4));
        }

        public void OnHitWindowEnd()
        {
            if (LastInteractedWindow.IsOpen)
            {
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsMoving", true);
                animator.SetInteger("RunAnimationIndex", RunAnimationIndex);
                animator.SetInteger("AnimIndex", AnimIndex);

                State = State.Chase;
                agent.speed = 0.1f;
                //animator.applyRootMotion = true;

                ChangeTarget(GameReferences.Instance.Player);
            }
        }

        public static int playertouches = 0;
        public static bool isBeingHit = false;

        public void OnPlayerTouch()
        {
            if (playertouches != 0)
                return;

            if (isBeingHit)
                return;

            OnHitPlayer();
        }

        public void OnPlayerStopTouch()
        {
            if (playertouches == 0)
                return;

            playertouches--;
        }

        private IEnumerator CheckStillColliding()
        {
            yield return new WaitForSeconds(1.5f);

            if (playertouches != 0 && !GameManager.Instance.GameEnded)
            {
                OnHitPlayer();
            }

            isBeingHit = false;
        }
    }
}