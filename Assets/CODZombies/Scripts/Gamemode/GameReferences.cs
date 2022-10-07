#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Objects.Window;
using UnityEngine;

namespace CODZombies.Scripts.Gamemode
{
    public class GameReferences : MonoBehaviourSingleton<GameReferences>
    {
        public Color CanBuyColor;
        public Color CannotBuyColor;

        [HideInInspector] public List<Window> Windows;

        [HideInInspector] public Transform Player;
        [SerializeField] private Transform DebugPlayer;

        [HideInInspector] public Transform PlayerHead;
        [SerializeField] private Transform DebugPlayerHead;

        public override void Awake()
        {
            base.Awake();

            Player = DebugPlayer;
            PlayerHead = DebugPlayerHead;

            Windows = FindObjectsOfType<Window>().ToList();
        }

        private IEnumerator Start()
        {
            while (FistVR.GM.CurrentPlayerBody == null)
                yield return null;

            Player = FistVR.GM.CurrentPlayerBody.transform;
            PlayerHead = FistVR.GM.CurrentPlayerBody.Head.transform;
        }

        public bool IsPlayerClose(Transform pos, float dist)
        {
            return Vector3.Distance(pos.position, Player.position) <= dist;
        }
    }
}

#endif