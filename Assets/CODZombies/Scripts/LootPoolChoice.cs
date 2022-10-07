using CODZombies.Scripts.Gamemode;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts
{
	public class LootPoolChoice : MonoBehaviour
	{
		public LootPool LootPool;
		public Text LootChoiceText;
		[HideInInspector] public bool IsEnabled;

		private void Awake()
		{
			LootChoiceText.text = LootPool.LootPoolTitle;
		}

		// Used by button
		public void ChangePoolToThis()
		{
			GameSettings.Instance.ChangeLootPool(this);
			IsEnabled = true;
		}
	}
}
