using System;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
	public int SuperSize = 5;

	public string FileName;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2))
		{
			Application.CaptureScreenshot(FileName + DateTime.Now.ToString("dd.mm.hh.mm.ss") + ".png", SuperSize);
		}
	}
}
