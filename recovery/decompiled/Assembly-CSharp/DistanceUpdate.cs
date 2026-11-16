using UnityEngine;
using UnityEngine.UI;

public class DistanceUpdate : MonoBehaviour
{
	public int DistanceMultiplier = 250;

	private Camera m_Camera;

	private Text m_Text;

	private void Start()
	{
		if (!m_Camera)
		{
			m_Camera = Camera.main;
		}
		if (!m_Text)
		{
			m_Text = GetComponent<Text>();
		}
	}

	private void Update()
	{
	}
}
