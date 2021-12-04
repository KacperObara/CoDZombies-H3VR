using System.Collections.Generic;
using UnityEngine;
using Atlas;

namespace CustomScripts
{
    public class GameReferences : MonoBehaviourSingleton<GameReferences>
    {
        public override void Awake()
        {
            base.Awake();

            Player = DebugPlayer;
            PlayerHead = DebugPlayerHead;
        }

        public Material CanBuyMat;
        public Material CannotBuyMat;

        public Color CanBuyColor;
        public Color CannotBuyColor;

        public List<Window> Windows;

        [HideInInspector] public Transform Player;
        [SerializeField] private Transform DebugPlayer;

        [HideInInspector] public Transform PlayerHead;
        [SerializeField] private Transform DebugPlayerHead;

        public CustomSceneInfo CustomScene;
        public Transform Respawn;

        private void Start()
        {
#if !UNITY_EDITOR // TODO define directives don't work for me for some reason

            if (FistVR.GM.CurrentPlayerBody != null)
                Player = FistVR.GM.CurrentPlayerBody.transform;

            if (FistVR.GM.CurrentPlayerBody != null)
                PlayerHead = FistVR.GM.CurrentPlayerBody.Head.transform;
#endif
        }

        public bool IsPlayerClose(Transform pos, float dist)
        {
            return Vector3.Distance(pos.position, Player.position) <= dist;
        }
    }
}