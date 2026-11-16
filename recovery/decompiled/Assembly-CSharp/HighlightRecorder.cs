using UnityEngine;

public class HighlightRecorder : MonoBehaviour
{
	private PhotonVoiceRecorder recorder;

	private Renderer rendererComp;

	private void Start()
	{
		recorder = transform.parent.GetComponent<PhotonVoiceRecorder>();
		if (recorder == null)
		{
			enabled = false;
			return;
		}
		rendererComp = GetComponent<Renderer>();
		if (rendererComp == null)
		{
			enabled = false;
		}
		else
		{
			rendererComp.enabled = false;
		}
	}

	private void Update()
	{
		if (!(recorder != null))
		{
		}
	}
}
