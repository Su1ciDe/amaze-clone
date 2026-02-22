using GamePlay;
using GridSystem;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

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
		private const string path = "Assets/ScriptableObjects/Levels";

		[MenuItem("Tools/Level Editor")]
		private static void ShowWindow()
		{
			var window = GetWindow<LevelEditor>();
			window.titleContent = new GUIContent("Level Editor");
			window.minSize = new Vector2(550, 750);
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
			// DrawRandomGrid();
			GUILayout.Space(10);
			DrawGrid();
			GUILayout.Space(10);
			GuiLine();

			// DrawRandomizer();
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
				Save();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void NewData()
		{
			var asset = ScriptableObject.CreateInstance<LevelDataSO>();
			var assetName = AssetDatabase.GenerateUniqueAssetPath(path + "/LevelData_000.asset");
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
			selectedColor = (ColorType)EditorGUILayout.EnumPopup(selectedColor, GUILayout.Width(100));

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

			for (int y = gridHeight - 1; y >= 0; y--)
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

		private void GuiLine(int height = 1)
		{
			var rect = EditorGUILayout.GetControlRect(false, height);
			rect.height = height;
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
			GUILayout.Space(10);
		}
	}
}