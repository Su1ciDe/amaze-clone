using GridSystem;
using ScriptableObjects;
using UnityEngine;

namespace GamePlay
{
	public class Level : MonoBehaviour
	{
		public int TotalMoves { get; set; }

		[SerializeField] private GridManager gridManager;
		public GridManager GridManager => gridManager;
		

		public void Load(LevelDataSO levelData)
		{
			gridManager.Setup(levelData);
		}

		public void Play()
		{
		}
	}
}