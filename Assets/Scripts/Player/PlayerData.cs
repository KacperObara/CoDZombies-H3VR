#if H3VR_IMPORTED
using FistVR;
namespace CustomScripts.Player
{
    public class PlayerData : MonoBehaviourSingleton<PlayerData>
    {
        public PowerUpIndicator DoublePointsPowerUpIndicator;
        public PowerUpIndicator InstaKillPowerUpIndicator;
        public PowerUpIndicator DeathMachinePowerUpIndicator;

        public float DamageModifier = 1f;
        public float MoneyModifier = 1f;

        public bool InstaKill = false;

        public bool DeadShotPerkActivated = false;
        public bool DoubleTapPerkActivated = false;
        public bool SpeedColaPerkActivated = false;

        private float _fastest = float.MinValue;
        private float _slowest = float.MaxValue;

        private float _timer;

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnRoundChanged -= OnRoundAdvance;
            RoundManager.OnRoundChanged += OnRoundAdvance;
        }

        private void OnDestroy()
        {
            RoundManager.OnRoundChanged -= OnRoundAdvance;
        }

        private void OnRoundAdvance()
        {
            GM.CurrentPlayerBody.HealPercent(1f);
        }

        // public FVRPlayerBody Player => GM.CurrentPlayerBody;
        // public FVRMovementManager MovementManager => GM.CurrentMovementManager;

        // private float PlayerSpeed
        // {
        //     get => Player.GetBodyMovementSpeed();
        //     set
        //     {
        //         MovementManager.SlidingSpeed = value;
        //         MovementManager.DashSpeed = value;
        //     }
        // }

        //private void Update()
        //{
        // if (Fastest > PlayerSpeed)
        //     Fastest = PlayerSpeed;
        //
        // if (Slowest < PlayerSpeed)
        //     Slowest = PlayerSpeed;
        //
        // if (Time.deltaTime > timer)
        // {
        //     timer = Time.deltaTime + 1f;
        //
        //     Debug.Log(PlayerSpeed + " " + Fastest + " " + Slowest);
        // }

        // if (GM.CurrentMovementManager.Hands[0].CurrentInteractable as FVRPhysicalObject != null)
        // {
        //     Debug.Log((GM.CurrentMovementManager.Hands[0].CurrentInteractable as FVRPhysicalObject).Size);
        // }
        //}
    }
}
#endif