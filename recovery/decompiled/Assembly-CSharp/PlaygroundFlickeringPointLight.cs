using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlaygroundFlickeringPointLight : MonoBehaviour
{
	public float flickerSpeed = 10f;

	public float minimumLight = 0.3f;

	public float maximumLight = 1f;

	private Light _thisLight;

	private float _targetLightValue;

	private void Start()
	{
		_thisLight = GetComponent<Light>();
		SetTargetLightValue();
	}

	private void Update()
	{
		if ((double)_thisLight.intensity < (double)_targetLightValue - 0.1 || (double)_thisLight.intensity > (double)_targetLightValue + 0.1)
		{
			_thisLight.intensity = Mathf.Lerp(_thisLight.intensity, _targetLightValue, flickerSpeed * Time.deltaTime);
		}
		else
		{
			SetTargetLightValue();
		}
	}

	private void SetTargetLightValue()
	{
		_targetLightValue = Random.Range(minimumLight, maximumLight);
	}
}
