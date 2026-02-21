using GamePlay;
using GridSystem;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "LevelData_000", menuName = "Amaze/Level Data", order = 0)]
	public class LevelDataSO : ScriptableObject
	{
		public NodeTypeMatrix GridCells;
		public Vector2Int GridSize;
		public ColorType ColorType;
	}
}