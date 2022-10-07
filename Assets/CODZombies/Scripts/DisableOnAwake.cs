using UnityEngine;

namespace CODZombies.Scripts
{
	public class DisableOnAwake : MonoBehaviour {
		private void Awake()
		{
			gameObject.SetActive(false);
		}
	}
}
