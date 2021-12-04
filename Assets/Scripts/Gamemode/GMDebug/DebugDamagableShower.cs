using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts.Gamemode.GMDebug
{
    /// <summary>
    /// Script for checking the damage of weapons/bullets
    /// </summary>
    public class DebugDamagableShower : MonoBehaviour, IFVRDamageable
    {
        public Text Text;
        public Text LowestText;
        public Text HighestText;

        public float DamageSum;
        public float LowestDam;
        public float HighestDam;

        public void ResetData()
        {
            DamageSum = 0;
            LowestDam = float.MaxValue;
            HighestDam = float.MinValue;

            Text.text = "Kinetic: " + 0;
            LowestText.text = "Lowest: " + 0;
            HighestText.text = "Highest: " + 0;
        }

        public void Damage(Damage dam)
        {
            DamageSum += dam.Dam_TotalKinetic;

            if (LowestDam < dam.Dam_TotalKinetic)
                LowestDam = dam.Dam_TotalKinetic;

            if (HighestDam > dam.Dam_TotalKinetic)
                HighestDam = dam.Dam_TotalKinetic;

            Text.text = "Kinetic: " + DamageSum;
            LowestText.text = "Lowest: " + LowestDam;
            HighestText.text = "Highest: " + HighestDam;
        }
    }
}