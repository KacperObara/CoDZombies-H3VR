#if H3VR_IMPORTED
using System.Collections.Generic;
using Atlas;
using UnityEngine;
namespace CustomScripts
{
    public class GameReferences : MonoBehaviourSingleton<GameReferences>
    {
        public Material CanBuyMat;
        public Material CannotBuyMat;

        public Color CanBuyColor;
        public Color CannotBuyColor;

        public List<Window> Windows;

        [HideInInspector] public Transform Player;
        [SerializeField] private Transform DebugPlayer;

        [HideInInspector] public Transform PlayerHead;
        [SerializeField] private Transform DebugPlayerHead;
        public Transform Respawn;

        public CustomSceneInfo CustomScene;
        public override void Awake()
        {
            base.Awake();

            Player = DebugPlayer;
            PlayerHead = DebugPlayerHead;
        }

        private void Start()
        {
#if !UNITY_EDITOR 
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

#endif