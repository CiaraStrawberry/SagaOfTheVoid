using UnityEngine;

public class F3DShipFX : MonoBehaviour
{
	public Transform[] LeftEngine;

	public Transform[] RightEngine;

	[Range(0f, 1f)]
	public float ServoCurrentPos;

	[Range(0f, 45f)]
	public float ServoLimit;

	public float ServoSpeed;

	private float servoPos;

	private float servoStep;

	private void Start()
	{
	}

	public static float ScaleRange(float value, float oldA, float oldB, float newA, float newB)
	{
		return (newB - newA) * (value - oldA) / (oldB - oldA) + newA;
	}

	private void Update()
	{
		float num = ScaleRange(Mathf.Clamp01(ServoCurrentPos), 0f, 1f, 0f, ServoLimit) - servoPos;
		servoStep = ServoSpeed * (num / 2f) * Time.deltaTime;
		for (int i = 0; i < LeftEngine.Length; i++)
		{
			LeftEngine[i].transform.localRotation *= Quaternion.Euler(servoStep, 0f, 0f);
			RightEngine[i].transform.localRotation *= Quaternion.Euler(servoStep, 0f, 0f);
		}
		servoPos += servoStep;
	}
}
