using System;

namespace Utilities
{
	[Serializable]
	public class Matrix<T>
	{
		public GridArray<T>[] Arrays;

		public T this[int x, int y]
		{
			get => Arrays[x][y];
			set => Arrays[x][y] = value;
		}

		public Matrix(int sizeX, int sizeY)
		{
			Arrays = new GridArray<T>[sizeX];
			for (int i = 0; i < sizeX; i++)
				Arrays[i] = new GridArray<T>(sizeY);
		}

		public int GetLength(int dimension)
		{
			return dimension switch
			{
				0 => Arrays.Length,
				1 => Arrays[0].Cells.Length,
				_ => 0
			};
		}
	}

	[Serializable]
	public class GridArray<T>
	{
		public T[] Cells;

		public T this[int index]
		{
			get => Cells[index];
			set => Cells[index] = value;
		}

		public GridArray(int size)
		{
			Cells = new T[size];
		}
	}
}