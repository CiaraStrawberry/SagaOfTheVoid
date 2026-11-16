using UnityEngine;

public class HighlightSpeaker : MonoBehaviour
{
	private PhotonVoiceSpeaker speaker;

	private Renderer rendererComp;

	private void Start()
	{
		speaker = transform.parent.GetComponent<PhotonVoiceSpeaker>();
		if (speaker == null)
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
		if (!(speaker != null))
		{
		}
	}
}
