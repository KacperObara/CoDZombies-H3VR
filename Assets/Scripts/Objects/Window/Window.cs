using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class WindowPlank : MonoBehaviour
    {
        public Plank Plank;
    }

    public class Window : MonoBehaviour
    {
        public Transform ZombieWaypoint;

        public List<WindowPlank> PlankSlots;

        public int PlanksRemain { get; set; } // Overcomplicated a little
        public bool IsOpen => PlanksRemain == 0;

        public List<Plank> AllPlanks;

        private AudioSource TearPlankAudio;

        private void Start()
        {
            TearPlankAudio = GetComponent<AudioSource>();
            RepairAll(false);
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
                AudioManager.Instance.BarricadeRepairSound.Play();
        }

        public void OnPlankTouch(Plank plank)
        {
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

            TearPlankAudio.Play();
            PlanksRemain--;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Plank>())
            {
                OnPlankTouch(other.GetComponent<Plank>());
            }
        }
    }
}