using System;
using UnityEngine;

namespace Utilities
{
	public static class Direction
	{
		public static Vector2Int GetDirection(Directions directions)
		{
			return directions switch
			{
				Directions.Up => new Vector2Int(0, 1),
				Directions.Down => new Vector2Int(0, -1),
				Directions.Left => new Vector2Int(-1, 0),
				Directions.Right => new Vector2Int(1, 0),
				_ => throw new ArgumentOutOfRangeException(nameof(directions), directions, null)
			};
		}
	}

	public enum Directions
	{
		Up,
		Down,
		Left,
		Right
	}
}