using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Keyboard Move")]
public class SgtKeyboardMove : MonoBehaviour
{
	[Tooltip("The key that needs to be held down to move")]
	public KeyCode Require;

	[Tooltip("The maximum speed of the movement")]
	public float Speed = 1f;

	[Tooltip("How sharp the movements are")]
	public float Dampening = 5f;

	[Tooltip("Dampen speed when near SgtTerrains?")]
	public bool CheckTerrains = true;

	[Tooltip("The minimum distance to the surface of a terrain you can be in world space")]
	public float RepelDistance = 0.1f;

	[Tooltip("The speed multiplier when near the surface of a terrain")]
	public float SlowSpeed = 0.1f;

	[Tooltip("How thick the dampening field around each terrain is in world space")]
	public float SlowThickness = 10f;

	private Vector3 offset;

	protected virtual void OnEnable()
	{
		offset = Vector3.zero;
	}

	protected virtual void Update()
	{
		float num = CalculateMaxSpeed();
		if (Require == KeyCode.None || Input.GetKey(Require))
		{
			offset += transform.forward * Input.GetAxisRaw("Vertical") * num * Time.deltaTime;
			offset += transform.right * Input.GetAxisRaw("Horizontal") * num * Time.deltaTime;
		}
		Vector3 position = transform.position;
		Vector3 target = transform.position + offset;
		Vector3 vector = SgtHelper.Dampen3(position, target, Dampening, Time.deltaTime, 0.1f);
		transform.position = vector;
		offset -= vector - position;
		RepelPositions();
	}

	private float CalculateMaxSpeed()
	{
		float num = Speed;
		Vector3 vector = transform.position + offset;
		if (CheckTerrains && SlowThickness > 0f)
		{
			for (int num2 = SgtTerrain.AllTerrains.Count - 1; num2 >= 0; num2--)
			{
				SgtTerrain sgtTerrain = SgtTerrain.AllTerrains[num2];
				float worldHeight = sgtTerrain.GetWorldHeight(vector);
				float t = Mathf.Clamp01(((vector - sgtTerrain.transform.position).magnitude - worldHeight) / SlowThickness);
				float num3 = Mathf.SmoothStep(SlowSpeed, Speed, t);
				if (num3 < num)
				{
					num = num3;
				}
			}
		}
		return num;
	}

	private void RepelPositions()
	{
		if (!CheckTerrains)
		{
			return;
		}
		Vector3 vector = transform.position + offset;
		for (int num = SgtTerrain.AllTerrains.Count - 1; num >= 0; num--)
		{
			SgtTerrain sgtTerrain = SgtTerrain.AllTerrains[num];
			float num2 = sgtTerrain.GetWorldHeight(vector) + RepelDistance;
			Vector3 vector2 = vector - sgtTerrain.transform.position;
			Vector3 vector3 = transform.position - sgtTerrain.transform.position;
			if (vector2.magnitude < num2)
			{
				vector = sgtTerrain.transform.position + vector2.normalized * num2;
			}
			if (vector3.magnitude < num2)
			{
				transform.position = sgtTerrain.transform.position + vector3.normalized * num2;
			}
		}
	}
}
