using UnityEngine;

public class BillBoardMe : MonoBehaviour
{
	public Camera m_Camera;

	private void Awake()
	{
		if (!m_Camera)
		{
			m_Camera = Camera.main;
		}
	}

	private void Update()
	{
		if ((bool)m_Camera)
		{
			Vector3 eulerAngles = Quaternion.LookRotation(m_Camera.transform.forward).eulerAngles;
			transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
		}
	}
}
