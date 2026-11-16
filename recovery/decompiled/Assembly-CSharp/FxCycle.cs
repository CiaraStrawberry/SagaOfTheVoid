using System;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using PlaygroundSplines;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FxCycle : MonoBehaviour
{
	public bool isSelfRunning = true;

	public bool looping = true;

	public List<PlaygroundFxCycleItem> particleFx;

	public PlaygroundParticlesC nextParticleFx;

	public PlaygroundParticlesC nextParticleSmokeFx;

	public PlaygroundParticlesC circleLights;

	public PlaygroundSpline camSpline;

	public float splineOffsetSpeed = 0.5f;

	public Transform camPivot;

	public Transform camRotation;

	public float camMovementSpeed = 1f;

	public float camRotationSpeed = 20f;

	public Color skyboxStartColor;

	public Color skyboxNextFxColor;

	public float skyboxNextFxColorTime = 0.1f;

	public float skyboxColorChangeTime = 1f;

	public GameObject pressSpaceText;

	public AudioSource particleBlastSound;

	private AudioClip[] particleBlastClips;

	private Color skyboxColorOnLoad;

	private int currentIndex = -1;

	private float camSplineTimeOffset;

	private float initialSplineSpeed;

	private float transitionTime = 1f;

	private float cameraPushBack = 10f;

	private float cameraPushBackForce = 15f;

	private float cameraPushBackRestoreSpeed = 1f;

	private Camera cam;

	private Transform camTransform;

	private ManipulatorObjectC repellentManipulator;

	private float camFov;

	private bool isSimulating;

	private bool inTransition;

	private bool canLoadNext = true;

	private void Start()
	{
		Cursor.visible = false;
		foreach (PlaygroundFxCycleItem item in particleFx)
		{
			item.particles.emit = false;
			item.particles.particleSystemGameObject.SetActive(value: false);
		}
		skyboxColorOnLoad = RenderSettings.skybox.GetColor("_Tint");
		RenderSettings.skybox.SetColor("_Tint", Color.black);
		cam = Camera.main;
		camTransform = cam.transform;
		camFov = cam.fieldOfView;
		repellentManipulator = PlaygroundC.GetManipulator(0);
		repellentManipulator.enabled = false;
		particleBlastClips = particleBlastSound.GetComponent<SoundFxArray>().sounds;
		camPivot.position = camSpline.GetPoint(0f);
		camRotation.rotation = Quaternion.LookRotation(-camRotation.position);
		StartCoroutine(FadeIn());
		if (isSelfRunning)
		{
			pressSpaceText.SetActive(value: false);
			StartCoroutine(Selfrunning());
		}
	}

	private IEnumerator FadeIn()
	{
		circleLights.splines[0].splineTransform.position = new Vector3(0f, 30f, 0f);
		circleLights.splines[1].splineTransform.position = new Vector3(0f, -30f, 0f);
		StartCoroutine(FadeInStartSkybox());
		if (isSelfRunning)
		{
			yield return new WaitForSeconds(6f);
		}
		float animTime = 5f;
		float startTime = Time.time;
		while (Time.time < startTime + animTime && currentIndex < 0)
		{
			circleLights.splines[0].splineTransform.position = new Vector3(0f, Mathf.Lerp(30f, 16f, Easing(2, (Time.time - startTime) / animTime)), 0f);
			circleLights.splines[1].splineTransform.position = new Vector3(0f, Mathf.Lerp(-30f, -16f, Easing(2, (Time.time - startTime) / animTime)), 0f);
			yield return null;
		}
		if (!isSelfRunning && currentIndex < 0)
		{
			pressSpaceText.SetActive(value: true);
		}
	}

	private IEnumerator FadeInStartSkybox()
	{
		Color currentColor = RenderSettings.skybox.GetColor("_Tint");
		float startTime = Time.time;
		while (Time.time < startTime + 11f && currentIndex < 0)
		{
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(currentColor, skyboxStartColor, (Time.time - startTime) / 11f));
			yield return null;
		}
	}

	private IEnumerator FadeOut()
	{
		Color currentColor = RenderSettings.skybox.GetColor("_Tint");
		Color endColor = Color.black;
		float animTime = 3f;
		float startTime = Time.time;
		while (Time.time < startTime + animTime)
		{
			circleLights.splines[0].splineTransform.position = new Vector3(0f, Mathf.Lerp(16f, 30f, Easing(1, (Time.time - startTime) / animTime)), 0f);
			circleLights.splines[1].splineTransform.position = new Vector3(0f, Mathf.Lerp(-16f, -30f, Easing(1, (Time.time - startTime) / animTime)), 0f);
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(currentColor, endColor, (Time.time - startTime) / animTime));
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		Application.Quit();
	}

	private void OnDisable()
	{
		RenderSettings.skybox.SetColor("_Tint", skyboxColorOnLoad);
	}

	private void Update()
	{
		CameraMovement();
		if (!isSelfRunning)
		{
			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(NextEffect(1));
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				StartCoroutine(NextEffect(-1));
			}
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			TimeScale(1);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			TimeScale(-1);
		}
		if (Input.GetKey(KeyCode.T))
		{
			TimeScale(0);
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void CameraMovement()
	{
		if (isSimulating)
		{
			if (initialSplineSpeed < 1f)
			{
				initialSplineSpeed += 0.1f * Time.deltaTime;
			}
			else
			{
				initialSplineSpeed = 1f;
			}
			camSplineTimeOffset += splineOffsetSpeed * initialSplineSpeed * Time.deltaTime;
			camSplineTimeOffset %= 1f;
			camPivot.position = Vector3.Lerp(camPivot.position, camSpline.GetPoint(camSplineTimeOffset), camMovementSpeed * Time.deltaTime);
			camRotation.rotation = Quaternion.Slerp(camRotation.rotation, Quaternion.LookRotation(-camRotation.position), camRotationSpeed * Time.deltaTime);
			if (!inTransition)
			{
				cameraPushBack = Mathf.Lerp(cameraPushBack, 0f, cameraPushBackRestoreSpeed * Time.deltaTime);
			}
		}
		camTransform.localPosition = new Vector3(0f, 0f, 0f - cameraPushBack);
		cam.fieldOfView = camFov + cameraPushBack;
	}

	private void NextEffect()
	{
		StartCoroutine(NextEffect(1));
	}

	private IEnumerator NextEffect(int increment)
	{
		if (!canLoadNext)
		{
			yield break;
		}
		isSimulating = true;
		inTransition = true;
		canLoadNext = false;
		pressSpaceText.SetActive(value: false);
		bool canContinue = looping || (!looping && currentIndex < particleFx.Count - 1);
		int previousIndex = currentIndex;
		bool previousOnlySourcePositioning = previousIndex >= 0 && particleFx[previousIndex].particles.onlySourcePositioning;
		bool previousOnlyLifetimePositioning = previousIndex >= 0 && particleFx[previousIndex].particles.onlyLifetimePositioning;
		bool previousTransitionBackToSource = previousIndex >= 0 && particleFx[previousIndex].particles.transitionBackToSource;
		if (previousOnlyLifetimePositioning)
		{
			for (int i = 0; i < particleFx[previousIndex].particles.particleCount; i++)
			{
				particleFx[previousIndex].particles.playgroundCache.changedByPropertyTarget[i] = true;
			}
		}
		if (previousIndex >= 0)
		{
			particleFx[previousIndex].particles.emit = false;
			particleFx[previousIndex].particles.onlySourcePositioning = false;
			particleFx[previousIndex].particles.onlyLifetimePositioning = false;
			particleFx[previousIndex].particles.transitionBackToSource = false;
			particleFx[previousIndex].particles.particleMaskTime = transitionTime * 0.75f;
			particleFx[previousIndex].particles.particleMask = particleFx[previousIndex].particles.particleCount;
			particleFx[previousIndex].particles.applyParticleMask = true;
		}
		float startTime;
		if (canContinue)
		{
			if (!isSelfRunning)
			{
				particleBlastSound.clip = particleBlastClips[UnityEngine.Random.Range(0, particleBlastClips.Length)];
				particleBlastSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				particleBlastSound.Play();
			}
			currentIndex = (currentIndex + increment) % particleFx.Count;
			if (currentIndex < 0)
			{
				currentIndex = particleFx.Count - 1;
			}
			StartCoroutine(NextSkyboxColor());
			nextParticleFx.Emit(setEmission: true);
			nextParticleSmokeFx.Emit(setEmission: true);
			repellentManipulator.enabled = true;
			yield return new WaitForSeconds(0.05f);
			startTime = Time.time;
			while (Time.time < startTime + transitionTime / 2f)
			{
				cameraPushBack = Mathf.Lerp(cameraPushBack, cameraPushBackForce, (Time.time - startTime) / (transitionTime / 2f));
				yield return null;
			}
			particleFx[currentIndex].particles.particleMaskTime = 0f;
			particleFx[currentIndex].particles.particleMask = particleFx[currentIndex].particles.particleCount;
			particleFx[currentIndex].particles.applyParticleMask = true;
			yield return null;
			repellentManipulator.enabled = false;
			if (particleFx[currentIndex].moveInOut)
			{
				particleFx[currentIndex].particles.particleSystemTransform.position = new Vector3(0f, -20f, 0f);
			}
			particleFx[currentIndex].particles.Emit(setEmission: true);
			while (!particleFx[currentIndex].particles.IsReady())
			{
				yield return null;
			}
			for (int j = 0; j < particleFx[currentIndex].particles.particleCount; j++)
			{
				particleFx[currentIndex].particles.playgroundCache.maskAlpha[j] = 0f;
				particleFx[currentIndex].particles.playgroundCache.isMasked[j] = true;
			}
			yield return null;
			particleFx[currentIndex].particles.particleMaskTime = transitionTime / 2f;
			particleFx[currentIndex].particles.particleMask = 0;
			nextParticleFx.Emit(setEmission: false);
			nextParticleSmokeFx.Emit(setEmission: false);
			inTransition = false;
			if (particleFx[currentIndex].moveInOut)
			{
				startTime = Time.time;
				while (Time.time < startTime + transitionTime / 2f)
				{
					particleFx[currentIndex].particles.particleSystemTransform.position = new Vector3(0f, Mathf.Lerp(-20f, 0f, Easing(2, (Time.time - startTime) / (transitionTime / 2f))), 0f);
					yield return null;
				}
				particleFx[currentIndex].particles.particleSystemTransform.position = Vector3.zero;
			}
		}
		startTime = Time.time;
		while (Time.time < startTime + transitionTime + 0.5f)
		{
			if (previousIndex >= 0 && particleFx[previousIndex].moveInOut)
			{
				particleFx[previousIndex].particles.particleSystemTransform.position = new Vector3(0f, Mathf.Lerp(0f, 20f, Easing(1, (Time.time - startTime) / (transitionTime / 2f))), 0f);
			}
			yield return null;
		}
		if (previousIndex >= 0 && canContinue)
		{
			yield return null;
			particleFx[previousIndex].particles.onlySourcePositioning = previousOnlySourcePositioning;
			particleFx[previousIndex].particles.onlyLifetimePositioning = previousOnlyLifetimePositioning;
			particleFx[previousIndex].particles.transitionBackToSource = previousTransitionBackToSource;
			particleFx[previousIndex].particles.particleSystemGameObject.SetActive(value: false);
		}
		if (canContinue)
		{
			canLoadNext = true;
		}
		else
		{
			StartCoroutine(FadeOut());
		}
	}

	private IEnumerator NextSkyboxColor()
	{
		Color currentColor = RenderSettings.skybox.GetColor("_Tint");
		float startTime = Time.time;
		while (Time.time < startTime + skyboxNextFxColorTime)
		{
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(currentColor, skyboxNextFxColor, (Time.time - startTime) / skyboxNextFxColorTime));
			yield return null;
		}
		startTime = Time.time;
		currentColor = RenderSettings.skybox.GetColor("_Tint");
		while (Time.time < startTime + skyboxColorChangeTime)
		{
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(currentColor, particleFx[currentIndex].colorProfile, (Time.time - startTime) / skyboxColorChangeTime));
			yield return null;
		}
	}

	private void TimeScale(int level)
	{
		if (level == 0)
		{
			Time.timeScale = 1f;
		}
		else
		{
			Time.timeScale = Mathf.Clamp(Time.timeScale + Time.deltaTime / Time.timeScale * ((float)level * 1f), 0.1f, 1f);
		}
	}

	private float Easing(int transitionType, float time)
	{
		return transitionType switch
		{
			0 => time, 
			1 => Mathf.Lerp(0f, 1f, 1f - Mathf.Cos(time * (float)Math.PI * 0.5f)), 
			2 => Mathf.Lerp(0f, 1f, Mathf.Sin(time * (float)Math.PI * 0.5f)), 
			_ => time, 
		};
	}

	private IEnumerator Selfrunning()
	{
		yield return new WaitForSeconds(15.5f);
		InvokeRepeating("NextEffect", 0f, 8f);
	}
}
