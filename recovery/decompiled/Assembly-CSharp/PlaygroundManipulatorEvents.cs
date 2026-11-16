using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

public class PlaygroundManipulatorEvents : MonoBehaviour
{
	public PlaygroundParticlesC bubbleParticles;

	public PlaygroundParticlesC popParticles;

	public PlaygroundParticlesC popTurbulenceParticles;

	public float maxRandomTurbulence = 1f;

	public float guiUpdateTime = 0.25f;

	public GUIText text;

	public GUIText popText;

	private ManipulatorObjectC manipulator;

	private Transform manipulatorTransform;

	private Camera mainCamera;

	private Color popTextColor;

	private int highestPop;

	private void Start()
	{
		manipulator = PlaygroundC.GetManipulator(0, bubbleParticles);
		manipulatorTransform = manipulator.transform.transform;
		manipulator.particleEventEnter += OnManipulatorEnter;
		manipulator.particleEventExit += OnManipulatorExit;
		manipulator.particleEventBirth += OnManipulatorBirth;
		manipulator.particleEventDeath += OnManipulatorDeath;
		manipulator.particleEventCollision += OnManipulatorCollision;
		mainCamera = Camera.main;
		popTextColor = popText.color;
		InvokeRepeating("UpdateGUI", 0f, guiUpdateTime);
	}

	private void Update()
	{
		manipulatorTransform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0f, 0f, 14f));
		if (Input.GetMouseButton(0))
		{
			StartCoroutine(Pop());
		}
	}

	private IEnumerator Pop()
	{
		List<PlaygroundEventParticle> manipulatorParticles = manipulator.GetParticles();
		foreach (PlaygroundEventParticle item in manipulatorParticles)
		{
			popParticles.Emit(item.position, item.velocity, item.color);
			for (int i = 0; i < 5; i++)
			{
				popTurbulenceParticles.Emit(item.position, item.velocity + RandomV3(), item.color);
			}
		}
		manipulator.KillAllParticles();
		if (manipulatorParticles.Count > 0 && manipulatorParticles.Count > highestPop)
		{
			popTextColor.a = 1f;
			popText.color = popTextColor;
			highestPop = manipulatorParticles.Count;
			popText.text = highestPop.ToString();
			Vector2 pixelOffset = mainCamera.WorldToScreenPoint(manipulatorTransform.position);
			pixelOffset.y += 50f;
			popText.pixelOffset = pixelOffset;
			int highestNow = highestPop;
			while (popTextColor.a > 0f && highestNow == highestPop)
			{
				popTextColor.a -= 0.5f * Time.deltaTime;
				popText.color = popTextColor;
				yield return null;
			}
		}
	}

	private void OnManipulatorEnter(PlaygroundEventParticle particle)
	{
	}

	private void OnManipulatorExit(PlaygroundEventParticle particle)
	{
	}

	private void OnManipulatorBirth(PlaygroundEventParticle particle)
	{
	}

	private void OnManipulatorDeath(PlaygroundEventParticle particle)
	{
	}

	private void OnManipulatorCollision(PlaygroundEventParticle particle)
	{
	}

	private Vector3 RandomV3()
	{
		return new Vector3(Random.Range(0f - maxRandomTurbulence, maxRandomTurbulence), Random.Range(0f - maxRandomTurbulence, maxRandomTurbulence), Random.Range(0f - maxRandomTurbulence, maxRandomTurbulence));
	}

	private void UpdateGUI()
	{
		text.text = "Bubbles in control: " + manipulator.particles.Count + "\nHighest pop: " + highestPop;
	}
}
