using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fiber.AudioSystem;
using GridSystem;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Utilities.AudioSystem;

namespace GamePlay
{
	public class Ball : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public Transform GetTransform() => transform;

		public bool IsMoving { get; private set; }

		public event UnityAction OnMoveComplete = () => { };

		private CancellationToken cancellationTokenOnDestroy;

		private void Awake()
		{
			cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		public async UniTask MoveToCell(List<GridCell> pathCells, Color trailColor)
		{
			if (IsMoving) return;

			IsMoving = true;
			var targetCell = pathCells[^1];

			// Remove from the current cell
			if (CurrentGridCell)
				CurrentGridCell.CurrentNode = null;

			transform.LookAt(targetCell.transform.position);
			transform.DOScale(new Vector3(transform.localScale.x * .5f, transform.localScale.y, transform.localScale.z * 1.5f), .2f);

			AudioManager.Instance.PlayAudio(AudioName.Slide).SetRandomPitch(0.9f, 1.1f);

			// Move to the target cell and color the cells along the path
			await transform.DOPath(pathCells.ConvertAll(cell => cell.transform.position).ToArray(), GameManager.Instance.GameSettingsSO.BallSpeed).SetSpeedBased(true).SetEase(Ease.InQuad)
				.OnWaypointChange(waypoint => pathCells[waypoint].ChangeColor(trailColor)).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationTokenOnDestroy);

			AudioManager.Instance.PlayAudio(AudioName.Wood).SetRandomPitch(0.9f, 1.1f);

			transform.DOKill();
			transform.DOScale(new Vector3(transform.localScale.x * 1.5f, transform.localScale.y, transform.localScale.z * 0.5f), .1f).OnComplete(() => { transform.DOScale(Vector3.one, .1f); });

			// Set new cell
			CurrentGridCell = targetCell;
			targetCell.CurrentNode = this;

			IsMoving = false;
			OnMoveComplete.Invoke();
		}
	}
}