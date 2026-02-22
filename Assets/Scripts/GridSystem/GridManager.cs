using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace GridSystem
{
	public class GridManager : MonoBehaviour
	{
		public int CellCount { get; private set; }
		public int TotalColoredCellCount { get; private set; }

		[Header("Grid Settings")]
		[SerializeField] private Vector2 cellSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		public float XSpacing => xSpacing;
		public float YSpacing => ySpacing;

		private Vector2Int size;
		private GridCellMatrix gridCells;
		private readonly List<Ball> balls = new List<Ball>();

		private float xOffset, yOffset;

		private ParticleSystem paintParticle;

		public static event UnityAction<int> OnMoveCompleted = moveCount => { };
		public static event UnityAction<int, int> OnPaintCompleted = (cellCount, totalPaintedCount) => { };

		private void OnEnable()
		{
			PlayerInputs.OnInput += HandleInput;
		}

		private void OnDisable()
		{
			PlayerInputs.OnInput -= HandleInput;
		}

		#region Setup

		public void Setup(LevelDataSO levelData)
		{
			SetupParticle(levelData.ColorType);

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
					cell.OnColored += OnCellColored;
					CellCount++;

					if (levelData.GridCells[x, y] == NodeType.Ball)
					{
						var ball = Instantiate(GameManager.Instance.PrefabsSO.BallPrefab);
						ball.transform.position = cell.transform.position;
						cell.SetNode(ball);
						balls.Add(ball);
						cell.ChangeColor(GameManager.Instance.ColorsSO.Colors[LevelManager.Instance.CurrentLevelData.ColorType]);
					}
					else if (levelData.GridCells[x, y] == NodeType.Wall)
					{
						var wall = Instantiate(GameManager.Instance.PrefabsSO.WallPrefab);
						wall.transform.position = cell.transform.position;
						cell.SetNode(wall);
						CellCount--;
					}
					else if (levelData.GridCells[x, y] == NodeType.None)
					{
						cell.DisableCell();
						CellCount--;
					}

					gridCells[x, y] = cell;
				}
			}
		}

		private void SetupParticle(ColorType colorType)
		{
			paintParticle = Instantiate(GameManager.Instance.PrefabsSO.PaintParticle);
			var main = paintParticle.main;
			main.startColor = GameManager.Instance.ColorsSO.Colors[colorType];
		}

		private void OnCellColored(GridCell cell)
		{
			paintParticle.transform.position = cell.transform.position;
			paintParticle.Play();

			cell.OnColored -= OnCellColored;
			TotalColoredCellCount++;

			if (TotalColoredCellCount >= CellCount)
				LevelManager.Instance.Win();
		}

		#endregion

		#region Movement

		private void HandleInput(Directions direction)
		{
			// Check if any ball is currently moving
			for (var i = 0; i < balls.Count; i++)
				if (balls[i].IsMoving)
					return;

			MoveAllBalls(direction).Forget();
		}

		private async UniTask MoveAllBalls(Directions direction)
		{
			var dir = Direction.GetDirection(direction);
			var moveTasks = new List<UniTask>();
			bool anyBallMoved = false;

			foreach (var ball in balls)
			{
				var currentPos = ball.CurrentGridCell.Coordinates;
				var pathCells = FindTargetPosition(currentPos, dir);

				if (pathCells[^1].Coordinates != currentPos)
				{
					anyBallMoved = true;
					moveTasks.Add(ball.MoveToCell(pathCells, GameManager.Instance.ColorsSO.Colors[LevelManager.Instance.CurrentLevelData.ColorType]));
				}
			}

			if (anyBallMoved)
			{
				LevelManager.Instance.CurrentLevel.TotalMoves++;
				OnMoveCompleted.Invoke(LevelManager.Instance.CurrentLevel.TotalMoves);
				await UniTask.WhenAll(moveTasks);

				OnPaintCompleted.Invoke(CellCount, TotalColoredCellCount);
			}
		}

		private List<GridCell> FindTargetPosition(Vector2Int startPos, Vector2Int direction)
		{
			var currentPos = startPos;
			var pathCells = new List<GridCell> { gridCells[currentPos.x, currentPos.y] };

			while (true)
			{
				var nextPos = currentPos + direction;

				// Check if next position is out of bounds
				if (nextPos.x < 0 || nextPos.x >= size.x || nextPos.y < 0 || nextPos.y >= size.y)
					return pathCells;

				var nextCell = gridCells[nextPos.x, nextPos.y];

				// Check if next cell is inactive (None type)
				if (!nextCell.IsActive)
					return pathCells;

				// Check if next cell has a wall or another ball
				if (nextCell.CurrentNode is Wall or Ball)
					return pathCells;

				// Move to next position
				currentPos = nextPos;
				pathCells.Add(gridCells[nextPos.x, nextPos.y]);
			}
		}

		#endregion
	}
}