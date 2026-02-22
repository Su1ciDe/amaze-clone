using Cysharp.Threading.Tasks;
using DG.Tweening;
using GridSystem;
using Interfaces;
using UnityEngine;
using Utilities;

namespace GamePlay
{
	public class Ball : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public Transform GetTransform() => transform;

		[Header("Movement Settings")]
		[SerializeField] private float moveDuration = 0.3f;

		public bool IsMoving { get; private set; }

		public async UniTask MoveToCell(GridCell targetCell)
		{
			if (IsMoving) return;

			IsMoving = true;

			// Remove from the current cell
			if (CurrentGridCell)
			{
				CurrentGridCell.CurrentNode = null;
			}

			// Move to the target cell
			await transform.DOMove(targetCell.transform.position, moveDuration).SetEase(Ease.InQuad).ToUniTask();

			// Set new cell
			CurrentGridCell = targetCell;
			targetCell.CurrentNode = this;

			IsMoving = false;
		}
	}
}