using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class FailPanelUI : PanelUI
	{
		[SerializeField] private TMP_Text txtLevelNo;
		[SerializeField] private Button btnRetry;

		private void Awake()
		{
			btnRetry.onClick.AddListener(Retry);

			LevelManager.OnLevelLose += Open;
		}
		
		private void OnDestroy()
		{
			LevelManager.OnLevelLose -= Open;
		}

		private void Retry()
		{
			LevelManager.Instance.RetryLevel();
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