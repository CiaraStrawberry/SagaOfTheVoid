using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
public class PlaygroundTrailParent : MonoBehaviour
{
	public PlaygroundTrails trailsReference;

	private GameObject _gameObject;

	private void Awake()
	{
		_gameObject = gameObject;
	}

	private void Update()
	{
		if (_gameObject != trailsReference.GetParentGameObject())
		{
			if (Application.isPlaying)
			{
				Object.Destroy(_gameObject);
			}
			else
			{
				Object.DestroyImmediate(_gameObject);
			}
		}
	}
}
