using UnityEngine;

namespace UI
{
	public abstract class PanelUI : MonoBehaviour
	{
		[SerializeField] protected RectTransform panel;

		public virtual void Open()
		{
			panel.gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			panel.gameObject.SetActive(false);
		}
	}
}