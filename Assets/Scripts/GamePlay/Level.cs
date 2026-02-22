using GridSystem;
using ScriptableObjects;
using UnityEngine;

namespace GamePlay
{
	public class Level : MonoBehaviour
	{
		public LevelDataSO CurrentLevelData { get; private set; }
		public int TotalMoves { get; set; }

		[SerializeField] private GridManager gridManager;
		public GridManager GridManager => gridManager;
		

		public void Load(LevelDataSO levelData)
		{
			CurrentLevelData = levelData;
			gridManager.Setup(levelData);
		}

		public void Play()
		{
		}
	}
}