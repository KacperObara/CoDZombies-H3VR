#if H3VR_IMPORTED
using System;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Player;
using FistVR;
using CustomScripts.Objects.Windows;
using UnityEngine;

namespace CustomScripts
{
    public class Window : MonoBehaviour
    {
        public static Action BarricadedEvent;

        public Transform ZombieWaypoint;

        public List<WindowPlank> PlankSlots;

        public List<Plank> AllPlanks;

        private AudioSource _tearPlankAudio;

        public int PlanksRemain { get; set; } // Overcomplicated a little

        public bool IsOpen
        {
            get { return PlanksRemain == 0; }
        }

        private void Start()
        {
            _tearPlankAudio = GetComponent<AudioSource>();
            RepairAll(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Plank>())
            {
                OnPlankTouch(other.GetComponent<Plank>());
            }
        }

        public void RepairAll(bool playSound = false)
        {
            for (int i = 0; i < PlankSlots.Count; i++)
            {
                PlankSlots[i].Plank = AllPlanks[i];
                AllPlanks[i].transform.position = PlankSlots[i].transform.position;
                AllPlanks[i].transform.rotation = PlankSlots[i].transform.rotation;
                AllPlanks[i].PhysicalObject.ForceBreakInteraction();
                AllPlanks[i].PhysicalObject.IsPickUpLocked = true;
            }

            PlanksRemain = PlankSlots.Count;

            if (playSound)
                AudioManager.Instance.Play(AudioManager.Instance.BarricadeRepairSound, .5f);
        }

        public void OnPlankTouch(Plank plank)
        {
            if (BarricadedEvent != null)
                BarricadedEvent.Invoke();

            if (PlayerData.Instance.SpeedColaPerkActivated)
            {
                RepairAll(true);
                return;
            }

            WindowPlank windowPlank = PlankSlots.FirstOrDefault(x => x.Plank == null);
            if (!windowPlank)
            {
                RepairAll(false); // temporary fix, sometimes last plank cant be connected
                return;
            }

            windowPlank.Plank = plank;

            plank.PhysicalObject.ForceBreakInteraction();
            plank.PhysicalObject.IsPickUpLocked = true;

            plank.OnRepairDrop(windowPlank.transform);

            PlanksRemain++;
        }

        public void OnPlankRipped()
        {
            WindowPlank windowPlank = PlankSlots.LastOrDefault(x => x.Plank != null);
            if (!windowPlank)
                return;


            Plank plank = windowPlank.Plank;
            plank.ReturnToRest();

            FVRPhysicalObject physicalObject = plank.GetComponent<FVRPhysicalObject>();
            physicalObject.IsPickUpLocked = false;

            windowPlank.Plank = null;

            _tearPlankAudio.Play();
            PlanksRemain--;
        }
    }
}
#endif