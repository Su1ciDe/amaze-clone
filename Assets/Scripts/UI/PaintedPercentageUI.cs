using DG.Tweening;
using GridSystem;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class PaintedPercentageUI : MonoBehaviour
	{
		[SerializeField] private Image imgFill;
		[SerializeField] private TMP_Text txtPercentage;

		private void Awake()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			GridManager.OnPaintCompleted += OnPaintCompleted;
		}

		private void OnDestroy()
		{
			GridManager.OnPaintCompleted -= OnPaintCompleted;
			LevelManager.OnLevelLoad -= OnLevelLoaded;
		}

		private void OnLevelLoaded()
		{
			imgFill.fillAmount = 0;
			txtPercentage.SetText("0 %");
		}

		private void OnPaintCompleted(int cellCount, int paintedCount)
		{
			var percent = paintedCount / (float)cellCount;
			imgFill.DOComplete();
			imgFill.DOFillAmount(percent, 0.2f).SetEase(Ease.InOutSine);
			txtPercentage.SetText(Mathf.RoundToInt(percent * 100) + " %");
		}
	}
}