using System.Collections;
using ParticlePlayground;
using UnityEngine;

public class CycleParticleSystems : MonoBehaviour
{
	public PlaygroundParticlesC[] particleSystems;

	public float transitionTime = 0.5f;

	private int _currentIndex;

	private int _previousIndex;

	private void Start()
	{
		DisableAll();
		GoTo(0);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Previous();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Next();
		}
	}

	public void DisableAll()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].particleSystemGameObject.SetActive(value: false);
		}
	}

	public void Next()
	{
		GoTo(_currentIndex + 1);
	}

	public void Previous()
	{
		GoTo(_currentIndex - 1);
	}

	public void GoTo(int index)
	{
		_previousIndex = _currentIndex;
		_currentIndex = index % particleSystems.Length;
		if (_currentIndex < 0)
		{
			_currentIndex = particleSystems.Length - 1;
		}
		StartCoroutine(GoToRoutine());
	}

	private IEnumerator GoToRoutine()
	{
		particleSystems[_previousIndex].applyParticleMask = true;
		particleSystems[_previousIndex].particleMaskTime = transitionTime;
		particleSystems[_previousIndex].particleMask = particleSystems[_previousIndex].particleCount;
		particleSystems[_currentIndex].particleSystemGameObject.SetActive(value: true);
		particleSystems[_currentIndex].applyParticleMask = true;
		particleSystems[_currentIndex].particleMaskTime = transitionTime;
		particleSystems[_currentIndex].particleMask = 0;
		yield return new WaitForSeconds(transitionTime);
		if (_previousIndex != _currentIndex)
		{
			particleSystems[_previousIndex].particleSystemGameObject.SetActive(value: false);
		}
	}
}
