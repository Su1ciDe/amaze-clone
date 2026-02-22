using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GridSystem
{
	public class GridCell : MonoBehaviour
	{
		public bool IsActive { get; set; } = true;
		public Vector2Int Coordinates { get; set; }
		public INode CurrentNode { get; set; }
		public bool IsColored { get; set; } = false;

		[Header("References")]
		[SerializeField] private MeshRenderer modelRenderer;
		[SerializeField] private Transform nodeHolder;

		public event UnityAction<GridCell> OnColored = gridCell => { };

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
			if (IsColored) return;

			IsColored = true;
			modelRenderer.material.color = color;

			OnColored.Invoke(this);
		}
	}
}