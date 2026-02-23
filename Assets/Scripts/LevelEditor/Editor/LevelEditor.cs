using System.Collections.Generic;
using System.Linq;
using GamePlay;
using GridSystem;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace LevelEditor.Editor
{
	public class LevelEditor : EditorWindow
	{
		private NodeTypeMatrix grid;
		private int gridWidth = 10;
		private int gridHeight = 10;

		private LevelDataSO levelSO;

		private NodeType selectedNodeType = NodeType.Cell;
		private ColorType selectedColor = ColorType.None;
		private Vector2 scrollPosition;

		private const int CELL_SIZE = 40;
		private const string PATH = "Assets/ScriptableObjects/Levels";

		private bool hasValidated;

		[MenuItem("Tools/Level Editor")]
		private static void ShowWindow()
		{
			var window = GetWindow<LevelEditor>();
			window.titleContent = new GUIContent("Level Editor");
			window.minSize = new Vector2(550, 600);
			window.Show();
		}

		private void OnEnable()
		{
			InitializeGrid();
		}

		private void InitializeGrid()
		{
			grid = new NodeTypeMatrix(gridWidth, gridHeight);
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					grid[x, y] = NodeType.Cell;
				}
			}

			hasValidated = false;
		}

		private void OnGUI()
		{
			GUILayout.Label("Level Editor", EditorStyles.boldLabel);

			DrawData();
			GUILayout.Space(10);
			GuiLine();
			DrawGridSettings();
			GUILayout.Space(10);
			DrawNodeTypeSelection();
			GUILayout.Space(10);
			DrawGrid();
			GUILayout.Space(10);
			GuiLine();
			GUILayout.Space(10);
			DrawRandomizer();
			GUILayout.Space(10);
			GuiLine();
			GUILayout.Space(5);
			DrawValidate();
		}

		#region Data

		private void DrawData()
		{
			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("New", GUILayout.Width(80), GUILayout.Height(20)))
			{
				NewData();
			}

			levelSO = (LevelDataSO)EditorGUILayout.ObjectField(levelSO, typeof(LevelDataSO), false);
			if (GUILayout.Button("Load", GUILayout.Width(80), GUILayout.Height(20)))
			{
				Load();
			}

			if (GUILayout.Button("Save", GUILayout.Width(80), GUILayout.Height(20)))
			{
				if (hasValidated)
				{
					Save();
				}
				else
				{
					EditorUtility.DisplayDialog("Validation", "Please validate the maze before saving.", "OK");
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		private void NewData()
		{
			var asset = ScriptableObject.CreateInstance<LevelDataSO>();
			var assetName = AssetDatabase.GenerateUniqueAssetPath(PATH + "/LevelData_000.asset");
			levelSO = asset;

			AssetDatabase.CreateAsset(asset, assetName);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
		}

		private void Load()
		{
			if (!levelSO) return;
			if (levelSO.GridCells.GetLength(0) == 0 || levelSO.GridCells.GetLength(1) == 0) return;

			gridWidth = levelSO.GridSize.x;
			gridHeight = levelSO.GridSize.y;
			grid = new NodeTypeMatrix(gridWidth, gridHeight);
			selectedColor = levelSO.ColorType;

			for (int x = 0; x < levelSO.GridCells.GetLength(0); x++)
			{
				for (int y = 0; y < levelSO.GridCells.GetLength(1); y++)
				{
					grid[x, y] = levelSO.GridCells[x, y];
				}
			}
		}

		private void Save()
		{
			if (!levelSO) return;

			EditorUtility.SetDirty(levelSO);

			levelSO.GridSize = new Vector2Int(gridWidth, gridHeight);
			levelSO.GridCells = grid;
			levelSO.ColorType = selectedColor;

			AssetDatabase.SaveAssets();
			EditorUtility.ClearDirty(levelSO);
		}

		#endregion

		#region Grid Settings

		private void DrawGridSettings()
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label("Width:", GUILayout.Width(50));
			int newWidth = EditorGUILayout.IntField(gridWidth, GUILayout.Width(50));

			GUILayout.Space(10);

			GUILayout.Label("Height:", GUILayout.Width(50));
			int newHeight = EditorGUILayout.IntField(gridHeight, GUILayout.Width(50));

			if (newWidth != gridWidth || newHeight != gridHeight)
			{
				gridWidth = Mathf.Clamp(newWidth, 1, 50);
				gridHeight = Mathf.Clamp(newHeight, 1, 50);
				InitializeGrid();
			}

			GUILayout.Space(50);
			GUILayout.Label("Paint Color:", GUILayout.Width(75));
			var previousColor = GUI.backgroundColor;
			GUI.color = GetColor(selectedColor);
			selectedColor = (ColorType)EditorGUILayout.EnumPopup(selectedColor, GUILayout.Width(100));
			GUI.color = previousColor;

			GUILayout.EndHorizontal();
		}

		private void DrawNodeTypeSelection()
		{
			GUILayout.Label("Selected Item Type: " + selectedNodeType.ToString(), EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			foreach (NodeType nodeType in System.Enum.GetValues(typeof(NodeType)))
			{
				if (nodeType == NodeType.Cell) continue;

				var previousColor = GUI.backgroundColor;
				if (selectedNodeType == nodeType)
					GUI.backgroundColor = Color.white;

				if (GUILayout.Button(nodeType.ToString(), GUILayout.Width(80), GUILayout.Height(30)))
					selectedNodeType = nodeType;

				GUI.backgroundColor = previousColor;
			}

			GUILayout.Label("Right click to clear", EditorStyles.miniLabel);

			GUILayout.EndHorizontal();
		}

		#endregion

		#region Grid

		private void DrawGrid()
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			for (int y = 0; y < gridHeight; y++)
			{
				GUILayout.BeginHorizontal();
				for (int x = 0; x < gridWidth; x++)
				{
					DrawCell(x, y);
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}

		private void DrawCell(int x, int y)
		{
			var cellType = grid[x, y];
			var previousColor = GUI.backgroundColor;
			var rect = GUILayoutUtility.GetRect(CELL_SIZE, CELL_SIZE, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));

			var e = Event.current;
			if (rect.Contains(e.mousePosition))
			{
				if (e.type is EventType.MouseDown or EventType.MouseDrag)
				{
					hasValidated = false;
					if (e.button == 0) // Left click
					{
						grid[x, y] = selectedNodeType;
						Repaint();
					}
					else if (e.button == 1) // Right click
					{
						grid[x, y] = NodeType.Cell;
						Repaint();
					}
				}
			}

			var currentStyle = new GUIStyle(GUI.skin.box) { normal = { background = new Texture2D(CELL_SIZE - 10, CELL_SIZE - 10) }, overflow = new RectOffset(-1, -1, -1, -1), };
			GUI.Box(rect, "", currentStyle);

			// Draw node type indicator
			DrawNodeTypeIndicator(rect, cellType);

			GUI.backgroundColor = previousColor;
		}

		private void DrawNodeTypeIndicator(Rect rect, NodeType nodeType)
		{
			switch (nodeType)
			{
				case NodeType.Ball:
					// Draw a circle
					DrawCircle(rect, Color.white);
					break;
				case NodeType.Wall:
					// Draw a square
					var squareRect = new Rect(rect.x + rect.width * 0.2f, rect.y + rect.height * 0.2f, rect.width * 0.6f, rect.height * 0.6f);
					EditorGUI.DrawRect(squareRect, Color.black);
					break;
				case NodeType.None:
					// Draw X
					DrawX(rect, Color.red);
					break;
			}
		}

		private void DrawCircle(Rect rect, Color color)
		{
			var center = rect.center;
			var radius = Mathf.Min(rect.width, rect.height) * 0.3f;

			Handles.color = color;
			Handles.DrawSolidDisc(center, Vector3.forward, radius);
		}

		private void DrawX(Rect rect, Color color)
		{
			var padding = rect.width * 0.2f;
			var topLeft = new Vector3(rect.x + padding, rect.y + padding, 0);
			var topRight = new Vector3(rect.xMax - padding, rect.y + padding, 0);
			var bottomLeft = new Vector3(rect.x + padding, rect.yMax - padding, 0);
			var bottomRight = new Vector3(rect.xMax - padding, rect.yMax - padding, 0);

			Handles.color = color;
			Handles.DrawLine(topLeft, bottomRight, 3f);
			Handles.DrawLine(topRight, bottomLeft, 3f);
		}

		#endregion

		#region Validation

		private void DrawValidate()
		{
			if (GUILayout.Button("Validate", GUILayout.Height(25)))
			{
				ValidateMaze();
			}
		}

		private bool ValidateMaze(bool showDialog = true)
		{
			// Collect ball positions and paintable cells
			var ballPositions = new List<Vector2Int>();
			var paintableCount = 0;

			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					var nodeType = grid[x, y];
					if (nodeType == NodeType.Ball)
					{
						ballPositions.Add(new Vector2Int(x, y));
						paintableCount++;
					}
					else if (nodeType == NodeType.Cell)
					{
						paintableCount++;
					}
				}
			}

			if (ballPositions.Count == 0)
			{
				if (showDialog)
					EditorUtility.DisplayDialog("Validation Failed", "The maze has no balls. Add at least one ball.", "OK");
				return false;
			}

			if (paintableCount == 0)
			{
				if (showDialog)
					EditorUtility.DisplayDialog("Validation Failed", "Maze has no paintable cells.", "OK");
				return false;
			}

			// BFS over states
			var cardinalDirs = new[]
			{
				Direction.GetDirection(Directions.Right), Direction.GetDirection(Directions.Left), Direction.GetDirection(Directions.Up), Direction.GetDirection(Directions.Down)
			};
			var initialState = ballPositions.ToList();
			var initialPainted = new HashSet<Vector2Int>(ballPositions);
			var visitedStates = new Dictionary<string, HashSet<Vector2Int>> { [MakeStateKey(initialState)] = new HashSet<Vector2Int>(initialPainted) };
			var queue = new Queue<(List<Vector2Int> state, HashSet<Vector2Int> painted)>();
			queue.Enqueue((initialState, initialPainted));
			var maxPainted = initialPainted.Count;

			while (queue.Count > 0)
			{
				var (state, painted) = queue.Dequeue();

				foreach (var dir in cardinalDirs)
				{
					var newState = new List<Vector2Int>();
					var newPainted = new HashSet<Vector2Int>(painted);

					foreach (var pos in state)
					{
						var landing = GetSlideEnd(pos, dir);
						newState.Add(landing);
						for (var p = pos;; p += dir)
						{
							newPainted.Add(p);
							if (p == landing)
								break;
						}
					}

					var key = MakeStateKey(newState);
					var shouldEnqueue = !visitedStates.TryGetValue(key, out var seenPainted);
					if (!shouldEnqueue && newPainted.Count > seenPainted.Count)
						shouldEnqueue = true;

					if (shouldEnqueue)
					{
						var merged = seenPainted != null ? new HashSet<Vector2Int>(seenPainted) : new HashSet<Vector2Int>();
						foreach (var c in newPainted)
							merged.Add(c);
						visitedStates[key] = merged;
						queue.Enqueue((newState, merged));
						if (merged.Count > maxPainted)
							maxPainted = merged.Count;
					}
				}
			}

			if (maxPainted >= paintableCount)
			{
				if (showDialog)
					EditorUtility.DisplayDialog("Validation OK", $"Maze is solvable. All {paintableCount} cells can be painted by the ball(s).", "OK");
				hasValidated = true;
				return true;
			}
			else
			{
				if (showDialog)
					EditorUtility.DisplayDialog("Validation Failed", $"Maze is not solvable. {maxPainted} of {paintableCount} cells are reachable.", "OK");
				return false;
			}
		}

		private static string MakeStateKey(List<Vector2Int> state)
		{
			return string.Join(" ", state.OrderBy(p => p.x).ThenBy(p => p.y).Select(p => p.x + "," + p.y));
		}

		// Returns the cell where a ball would stop
		private Vector2Int GetSlideEnd(Vector2Int pos, Vector2Int dir)
		{
			var current = pos;
			while (true)
			{
				var next = current + dir;
				if (next.x < 0 || next.x >= gridWidth || next.y < 0 || next.y >= gridHeight)
					return current;

				var nextType = grid[next.x, next.y];
				if (nextType is NodeType.None or NodeType.Wall or NodeType.Ball)
					return current;

				current = next;
			}
		}

		#endregion

		#region Randomizer

		private float randomPercent = 0.5f;
		private int maxAttempts = 500;

		private void DrawRandomizer()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Try Attempts: ", EditorStyles.boldLabel);
			maxAttempts = EditorGUILayout.IntField(maxAttempts);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Wall Percentage: " + Mathf.RoundToInt(randomPercent * 100) + "%", EditorStyles.boldLabel);
			randomPercent = GUILayout.HorizontalSlider(randomPercent, 0f, 1f);
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Generate Random Maze", GUILayout.Height(25)))
			{
				GenerateRandomMaze();
			}
		}

		private void GenerateRandomMaze()
		{
			var random = new System.Random();
			bool isSolvable = false;

			for (int attempt = 0; attempt < maxAttempts && !isSolvable; attempt++)
			{
				// Initialize grid
				for (int x = 0; x < gridWidth; x++)
				{
					for (int y = 0; y < gridHeight; y++)
					{
						grid[x, y] = NodeType.Cell;
					}
				}

				// Randomly place walls to create maze-like patterns
				int wallCount = (int)(gridWidth * gridHeight * randomPercent);
				for (int i = 0; i < wallCount; i++)
				{
					int x = random.Next(0, gridWidth);
					int y = random.Next(0, gridHeight);

					// Don't place wall at bottom left corner (ball position)
					if (x == 0 && y == 0)
						continue;

					grid[x, y] = NodeType.Wall;
				}

				// Place ball at bottom left corner
				grid[0, 0] = NodeType.Ball;

				// Check if solvable
				isSolvable = ValidateMaze(false);
			}

			if (!isSolvable)
			{
				EditorUtility.DisplayDialog("Generation Failed", $"Could not generate a solvable maze after {maxAttempts} attempts. Try again or adjust grid size.", "OK");
			}
			else
			{
				hasValidated = true;
			}

			Repaint();
		}

		#endregion

		private Color GetColor(ColorType itemType)
		{
			return itemType switch
			{
				ColorType.Green => Color.green,
				ColorType.Blue => Color.blue,
				ColorType.Red => Color.red,
				ColorType.Pink => new Color(1f, 0.55f, 0.85f),
				ColorType.Purple => new Color(0.5f, 0f, 0.5f),
				ColorType.Yellow => Color.yellow,
				_ => Color.white
			};
		}

		private void GuiLine(int height = 1)
		{
			var rect = EditorGUILayout.GetControlRect(false, height);
			rect.height = height;
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
			GUILayout.Space(10);
		}
	}
}