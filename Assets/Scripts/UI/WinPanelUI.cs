using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class WinPanelUI : PanelUI
	{
		[SerializeField] private TMP_Text txtLevelNo;
		[SerializeField] private Button btnContinue;

		private void Awake()
		{
			btnContinue.onClick.AddListener(Win);

			LevelManager.OnLevelWin += Open;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelWin -= Open;
		}

		private void Win()
		{
			LevelManager.Instance.LoadNextLevel();
			Close();
		}

		private void SetLevelNo()
		{
			txtLevelNo.SetText("LEVEL " + LevelManager.Instance.LevelNo.ToString());
		}

		public override void Open()
		{
			SetLevelNo();
			base.Open();
		}
	}
}