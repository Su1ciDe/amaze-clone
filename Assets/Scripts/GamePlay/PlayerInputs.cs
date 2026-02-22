using Managers;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace GamePlay
{
	public class PlayerInputs : MonoBehaviour
	{
		public bool CanInput { get; set; }

		private Vector3 mouseStartPos;
		private float swipeThreshold;

		public static event UnityAction<Directions> OnInput = _ => { };

		private void Awake()
		{
			swipeThreshold = Screen.width * 0.2f;

			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelLose += OnLevelLost;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelUnload += OnLevelUnloaded;
		}

		private void Update()
		{
			Controls();
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelLose -= OnLevelLost;
			LevelManager.OnLevelWin -= OnLevelWon;
			LevelManager.OnLevelUnload -= OnLevelUnloaded;
		}

		private void Controls()
		{
			if (!CanInput) return;

			if (Input.GetMouseButtonDown(0))
			{
				mouseStartPos = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(0))
			{
				var swipe = Input.mousePosition - mouseStartPos;
				if (swipe.magnitude > swipeThreshold)
				{
					var dir = swipe.normalized;
					var direction = GetSwipeDirection(dir);
					OnInput?.Invoke(direction);
				}
			}
		}

		private Directions GetSwipeDirection(Vector3 direction)
		{
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
			{
				return direction.x > 0 ? Directions.Right : Directions.Left;
			}
			else
			{
				return direction.y > 0 ? Directions.Down : Directions.Up;
			}
		}

		private void OnLevelStarted()
		{
			CanInput = true;
		}

		private void OnLevelWon()
		{
			CanInput = false;
		}

		private void OnLevelLost()
		{
			CanInput = false;
		}

		private void OnLevelUnloaded()
		{
			CanInput = false;
		}
	}
}