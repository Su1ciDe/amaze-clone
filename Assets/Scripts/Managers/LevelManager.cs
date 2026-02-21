using System.Linq;
using GamePlay;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Managers
{
	public class LevelManager : Singleton<LevelManager>
	{
		public int LevelNo
		{
			get => PlayerPrefs.GetInt(PlayerPrefNames.LEVEL_NO, 1);
			set => PlayerPrefs.SetInt(PlayerPrefNames.LEVEL_NO, value);
		}

		public LevelDataSO CurrentLevelData { get; private set; }
		public Level CurrentLevel { get; private set; }

		[SerializeField] private LevelDataSO[] levels;

		private int currentLevelIndex;

		public static event UnityAction OnLevelLoad;
		public static event UnityAction OnLevelUnload;
		public static event UnityAction OnLevelStart;
		public static event UnityAction OnLevelWin;
		public static event UnityAction OnLevelLose;

		private void Start()
		{
			LoadLevel();
		}

		private void LoadLevel()
		{
			currentLevelIndex = (LevelNo - 1) % levels.Length;

			CurrentLevelData = levels[currentLevelIndex];
			CurrentLevel = Instantiate(GameManager.Instance.PrefabsSO.LevelPrefab);
			CurrentLevel.Load(CurrentLevelData);
			OnLevelLoad?.Invoke();

			StartLevel();
		}

		private void StartLevel()
		{
			CurrentLevel.Play();
			OnLevelStart?.Invoke();
		}

		public void RetryLevel()
		{
			UnloadLevel();

			LoadLevel();
		}

		public void LoadNextLevel()
		{
			UnloadLevel();

			LevelNo++;
			LoadLevel();
		}

		private void UnloadLevel()
		{
			OnLevelUnload?.Invoke();
			Destroy(CurrentLevel.gameObject);
		}

		public void Win()
		{
			OnLevelWin?.Invoke();
		}

		public void Lose()
		{
			OnLevelLose?.Invoke();
		}

		[ContextMenu("Populate Levels")]
		private void PopulateLevels()
		{
			const string path = "Assets/ScriptableObjects/Levels";
			levels = EditorUtilities.LoadAllAssetsFromPath<LevelDataSO>(path).ToArray();
		}
	}
}