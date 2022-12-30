using CODZombies.Scripts.Managers;
using UnityEngine;

namespace CODZombies.Scripts
{
	public class StartingArea : MonoBehaviour
	{
		public AudioSource StartingMusic;

		private void Awake()
		{
			RoundManager.OnGameStarted += OnGameStart;
		}

		public void OnGameStart()
		{
			StartingMusic.Stop();
			//Destroy(gameObject);
		}

		private void OnDestroy()
		{
			RoundManager.OnGameStarted -= OnGameStart;
		}
	}
}
