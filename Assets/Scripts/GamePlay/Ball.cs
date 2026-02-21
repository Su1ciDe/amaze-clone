using GridSystem;
using Interfaces;
using UnityEngine;

namespace GamePlay
{
	public class Ball : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public Transform GetTransform() => transform;
	}
}