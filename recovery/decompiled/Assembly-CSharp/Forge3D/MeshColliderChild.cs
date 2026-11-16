using UnityEngine;

namespace Forge3D;

public class MeshColliderChild : MonoBehaviour
{
	private Vector3 positionDiff = Vector3.zero;

	private Vector3 scaleDiff = Vector3.zero;

	private Transform transf;

	public Transform parent;

	private void Start()
	{
		transf = transform;
		if (parent != null)
		{
			Install(parent);
		}
	}

	public void Install(Transform _parent)
	{
		if (transf == null)
		{
			transf = transform;
		}
		if (_parent == null)
		{
			Debug.Log("PARENT NULL");
			return;
		}
		parent = _parent;
		transf.parent = _parent.parent;
		positionDiff = _parent.position - transf.position;
		scaleDiff = new Vector3(_parent.localScale.x / transf.localScale.x, _parent.localScale.y / transf.localScale.y, _parent.localScale.z / transf.localScale.z);
	}

	private void LateUpdate()
	{
		if (!(parent == null))
		{
			transf.position = parent.position - positionDiff;
			transf.localEulerAngles = parent.localEulerAngles;
			transf.localScale = new Vector3(parent.localScale.x / scaleDiff.x, parent.localScale.y / scaleDiff.y, parent.localScale.z / scaleDiff.z);
			if (transf.gameObject.activeSelf && !parent.gameObject.activeSelf)
			{
				transf.gameObject.SetActive(value: false);
			}
			else if (transf.gameObject.activeSelf && !parent.gameObject.activeSelf)
			{
				transf.gameObject.SetActive(value: true);
			}
		}
	}
}
