using GridSystem;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class InGameUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text txtLevelNo;
		[SerializeField] private TMP_Text txtMoveCount;
		[SerializeField] private Button btnRestart;

		private void Awake()
		{
			btnRestart.onClick.AddListener(Restart);
			SetLevelNo(LevelManager.Instance.LevelNo);

			LevelManager.OnLevelLoad += OnLevelLoaded;
			GridManager.OnMoveCompleted += SetMoveCount;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			GridManager.OnMoveCompleted -= SetMoveCount;
		}

		private void OnLevelLoaded()
		{
			SetLevelNo(LevelManager.Instance.LevelNo);
			SetMoveCount(0);
		}

		public void SetLevelNo(int levelNo)
		{
			if (txtLevelNo)
				txtLevelNo.SetText("LEVEL " + levelNo.ToString());
		}

		public void SetMoveCount(int moveCount)
		{
			if (txtMoveCount)
				txtMoveCount.SetText(moveCount.ToString());
		}

		private void Restart()
		{
			LevelManager.Instance.RetryLevel();
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}