using GamePlay;
using Utilities;

namespace GridSystem
{
	[System.Serializable]
	public class NodeTypeMatrix : Matrix<NodeType>
	{
		public NodeTypeMatrix(int sizeX, int sizeY) : base(sizeX, sizeY)
		{
		}
	}
}