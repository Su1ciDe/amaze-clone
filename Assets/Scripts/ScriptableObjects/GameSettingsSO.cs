using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Settings", menuName = "Amaze/Settings", order = 0)]
	public class GameSettingsSO : ScriptableObject
	{
		public float BallSpeed = 10;
	}
}