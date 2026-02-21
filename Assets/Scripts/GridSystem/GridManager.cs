using GamePlay;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace GridSystem
{
	public class GridManager : MonoBehaviour
	{
		[SerializeField] private GridCellMatrix gridCells;
		public GridCellMatrix GridCells => gridCells;

		[Header("Grid Settings")]
		[SerializeField] private Vector2 cellSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		public float XSpacing => xSpacing;
		public float YSpacing => ySpacing;

		private Vector2Int size;

		private float xOffset, yOffset;

		#region Setup

		public void Setup(LevelDataSO levelData)
		{
			size = levelData.GridSize;
			gridCells = new GridCellMatrix(size.x, size.y);

			xOffset = (cellSize.x * size.x + xSpacing * (size.x - 1)) / 2f - cellSize.x / 2f;
			yOffset = (cellSize.y * size.y + ySpacing * (size.y - 1)) / 2f - cellSize.y / 2f;

			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var cell = Instantiate(GameManager.Instance.PrefabsSO.GridCellPrefab, transform);
					cell.Setup(x, y);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (cellSize.x + xSpacing) - xOffset, 0, -y * (cellSize.y + ySpacing) + yOffset);

					if (levelData.GridCells[x, y] == NodeType.Ball)
					{
						var ball = Instantiate(GameManager.Instance.PrefabsSO.BallPrefab);
						ball.transform.position = cell.transform.position;
						cell.SetNode(ball);
					}
					else if (levelData.GridCells[x, y] == NodeType.Wall)
					{
						var wall = Instantiate(GameManager.Instance.PrefabsSO.WallPrefab);
						wall.transform.position = cell.transform.position;
						cell.SetNode(wall);
					}
					else if (levelData.GridCells[x, y] == NodeType.None)
					{
						cell.DisableCell();
					}

					gridCells[x, y] = cell;
				}
			}
		}

		#endregion
	}
}