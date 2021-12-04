using System;
using UnityEngine;

namespace CustomScripts
{
    // Drop plank if player leaves from window
    public class DropPlankZone : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            Plank plank = other.GetComponent<Plank>();
            if (plank)
            {
                plank.PhysicalObject.ForceBreakInteraction();
                plank.ReturnToRest();
            }
        }
    }
}