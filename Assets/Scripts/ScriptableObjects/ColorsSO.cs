using AYellowpaper.SerializedCollections;
using GamePlay;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Colors", menuName = "Amaze/Colors", order = 0)]
	public class ColorsSO : ScriptableObject
	{
		public SerializedDictionary<ColorType, Color> Colors = new SerializedDictionary<ColorType, Color>();
	}
}