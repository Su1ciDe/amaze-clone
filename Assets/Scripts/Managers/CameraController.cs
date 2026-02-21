using UnityEngine;

namespace Managers
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;
		[SerializeField] private float padding = 1f;

		private void Awake()
		{
			if (mainCamera == null)
				mainCamera = Camera.main;

			LevelManager.OnLevelLoad += FitCameraToGrid;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoad -= FitCameraToGrid;
		}

		private void FitCameraToGrid()
		{
			var level = LevelManager.Instance.CurrentLevel;
			if (!level) return;

			var levelData = level.CurrentLevelData;
			if (!levelData) return;

			var gridManager = level.GridManager;
			if (!gridManager) return;

			var gridWidth = levelData.GridSize.x + gridManager.XSpacing * (levelData.GridSize.x - 1);
			var gridHeight = levelData.GridSize.y + gridManager.YSpacing * (levelData.GridSize.y - 1);

			var requiredWidth = gridWidth + padding * 2;
			var requiredHeight = gridHeight + padding * 2;

			// Calculate orthographic size to fit the grid
			var aspectRatio = (float)Screen.width / Screen.height;
			var heightBasedSize = requiredHeight / 2f;
			var widthBasedSize = requiredWidth / (2f * aspectRatio);

			// Use the larger size to ensure everything fits
			mainCamera.orthographicSize = Mathf.Max(heightBasedSize, widthBasedSize);

			// Center camera on the grid center
			var cameraPosition = mainCamera.transform.position;
			mainCamera.transform.position = cameraPosition;
		}
	}
}