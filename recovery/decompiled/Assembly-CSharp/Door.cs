using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
	public float translateValue;

	public float easeTime;

	public OTween.EaseType ease;

	public float waitTime;

	private Vector3 StartlocalPos;

	private Vector3 endlocalPos;

	private void Start()
	{
		StartlocalPos = transform.localPosition;
		gameObject.isStatic = false;
	}

	public void OpenDoor()
	{
		OTween.ValueTo(gameObject, ease, 0f, 0f - translateValue, easeTime, 0f, "StartOpen", "UpdateOpenDoor", "EndOpen");
		GetComponent<AudioSource>().Play();
	}

	private void UpdateOpenDoor(float f)
	{
		Vector3 vector = transform.TransformDirection(new Vector3(1f, 0f, 0f));
		transform.localPosition = StartlocalPos + vector * f;
	}

	private void UpdateCloseDoor(float f)
	{
		Vector3 vector = transform.TransformDirection(new Vector3(0f - f, 0f, 0f));
		transform.localPosition = endlocalPos - vector;
	}

	private void EndOpen()
	{
		endlocalPos = transform.localPosition;
		StartCoroutine(WaitToClose());
	}

	private IEnumerator WaitToClose()
	{
		yield return new WaitForSeconds(waitTime);
		OTween.ValueTo(gameObject, ease, 0f, translateValue, easeTime, 0f, "StartClose", "UpdateCloseDoor", "EndClose");
		GetComponent<AudioSource>().Play();
	}
}
