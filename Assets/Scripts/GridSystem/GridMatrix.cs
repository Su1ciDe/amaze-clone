using Utilities;

namespace GridSystem
{
	[System.Serializable]
	public class GridCellMatrix : Matrix<GridCell>
	{
		public GridCellMatrix(int sizeX, int sizeY) : base(sizeX, sizeY)
		{
		}
	}
}