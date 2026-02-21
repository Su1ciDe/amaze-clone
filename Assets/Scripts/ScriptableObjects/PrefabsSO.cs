using GamePlay;
using GridSystem;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Prefabs", menuName = "Amaze/Prefabs", order = 0)]
	public class PrefabsSO : ScriptableObject
	{
		public Ball BallPrefab;
		public GridCell GridCellPrefab;
		public Wall WallPrefab;
		public Level LevelPrefab;
	}
}