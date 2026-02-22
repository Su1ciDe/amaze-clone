using GamePlay;
using Interfaces;
using UnityEngine;

namespace GridSystem
{
	public class GridCell : MonoBehaviour
	{
		public bool IsActive { get; set; } = true;
		public Vector2Int Coordinates { get; set; }
		public INode CurrentNode { get; set; }

		[Header("References")]
		[SerializeField] private MeshRenderer modelRenderer;
		[SerializeField] private Transform nodeHolder;

		public void Setup(int x, int y)
		{
			Coordinates = new Vector2Int(x, y);
		}

		public void SetNode(INode node)
		{
			CurrentNode = node;
			node.CurrentGridCell = this;
			node.GetTransform().SetParent(nodeHolder);
		}

		public void DisableCell()
		{
			IsActive = false;
			modelRenderer.gameObject.SetActive(false);
		}

		public void ChangeColor(Color color)
		{
			modelRenderer.material.color = color;
		}
	}
}