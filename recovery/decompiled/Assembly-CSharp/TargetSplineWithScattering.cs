using System;
using ParticlePlayground;
using PlaygroundSplines;
using UnityEngine;

public class TargetSplineWithScattering : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public PlaygroundSpline spline;

	[Range(0.01f, 10f)]
	public float emitTime = 0.1f;

	public Color32 color = Color.white;

	public bool applyPositionScatter = true;

	public MINMAXVECTOR3METHOD positionScatterMethod;

	public Vector3 positionScatterMin = new Vector3(0f, -1f);

	public Vector3 positionScatterMax = new Vector3(0f, 1f);

	public bool applyScatterLifetimeMultiplier = true;

	public AnimationCurve positionScatterLifetimeMultiplier = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	private float _emitTimer;

	private Vector3[] _scatterPosition;

	private void Awake()
	{
		particles.scriptedEmissionIndex = 0;
		particles.gravity = Vector3.zero;
		particles.applyInitialVelocity = false;
		GenerateScatter();
	}

	private void Update()
	{
		if (!particles.IsReady())
		{
			return;
		}
		_emitTimer += Time.deltaTime / emitTime;
		if (_emitTimer >= 1f)
		{
			Vector3 point = spline.GetPoint(0);
			particles.Emit(point, Vector3.zero, color);
			_emitTimer = 0f;
		}
		if (applyPositionScatter && particles.particleCount != _scatterPosition.Length)
		{
			GenerateScatter();
		}
		for (int i = 0; i < particles.particleCount; i++)
		{
			if (applyPositionScatter && particles.playgroundCache.simulate.Length != _scatterPosition.Length)
			{
				break;
			}
			if (particles.playgroundCache.simulate[i])
			{
				float num = Mathf.Min(particles.playgroundCache.life[i] / (particles.lifetime - particles.playgroundCache.lifetimeSubtraction[i]), 0.99f);
				Vector3 point2 = spline.GetPoint(num);
				if (applyPositionScatter)
				{
					point2 += ((!applyScatterLifetimeMultiplier) ? _scatterPosition[i] : (_scatterPosition[i] * positionScatterLifetimeMultiplier.Evaluate(num)));
				}
				particles.ParticlePosition(i, point2);
			}
		}
	}

	private void GenerateScatter()
	{
		if (!applyPositionScatter)
		{
			return;
		}
		_scatterPosition = new Vector3[particles.particleCount];
		System.Random random = new System.Random();
		for (int i = 0; i < _scatterPosition.Length; i++)
		{
			if (positionScatterMethod == MINMAXVECTOR3METHOD.Rectangular)
			{
				ref Vector3 reference = ref _scatterPosition[i];
				reference = PlaygroundParticlesC.RandomRange(random, positionScatterMin, positionScatterMax);
			}
			else if (positionScatterMethod == MINMAXVECTOR3METHOD.RectangularLinear)
			{
				ref Vector3 reference2 = ref _scatterPosition[i];
				reference2 = Vector3.Lerp(positionScatterMin, positionScatterMax, (float)i * 1f / ((float)_scatterPosition.Length * 1f));
			}
			else if (positionScatterMethod == MINMAXVECTOR3METHOD.Spherical)
			{
				ref Vector3 reference3 = ref _scatterPosition[i];
				reference3 = PlaygroundParticlesC.RandomRangeSpherical(random, positionScatterMin.x, positionScatterMax.x);
			}
			else if (positionScatterMethod == MINMAXVECTOR3METHOD.SphericalLinear)
			{
				ref Vector3 reference4 = ref _scatterPosition[i];
				reference4 = PlaygroundParticlesC.RandomRangeSpherical(random, positionScatterMin.x, positionScatterMax.x, (float)i * 1f / ((float)_scatterPosition.Length * 1f));
			}
		}
	}
}
