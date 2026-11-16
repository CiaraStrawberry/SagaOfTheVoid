using System;
using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSpiralBetweenTwoPoints : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public Transform start;

	public Transform end;

	[Range(1f, 100f)]
	public float swirlIterations = 10f;

	public float radius = 1f;

	public AnimationCurve radiusTimeScale = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	public Color32 color = Color.white;

	private bool _ready;

	private Vector3[] _spiralInterpolations;

	private Vector3 _startPos;

	private Vector3 _endPos;

	private Vector3 _lookDirection;

	private float _r;

	private float _i;

	private void OnEnable()
	{
		if (!(particles == null) && !(start == null) && !(end == null))
		{
			particles.source = SOURCEC.Script;
			particles.onlySourcePositioning = true;
			GenerateSpiralInterpolations();
			_ready = true;
		}
	}

	private void Update()
	{
		if (!_ready || !particles.IsReady() || particles.particleCount < 2)
		{
			return;
		}
		if (_spiralInterpolations.Length != particles.particleCount || _startPos != start.position || _endPos != end.position || !Mathf.Approximately(_i, swirlIterations) || !Mathf.Approximately(_r, radius))
		{
			GenerateSpiralInterpolations();
		}
		for (int i = 0; i < particles.particleCount; i++)
		{
			if (_spiralInterpolations.Length != particles.playgroundCache.simulate.Length)
			{
				break;
			}
			if (!particles.playgroundCache.simulate[i])
			{
				particles.Emit(_spiralInterpolations[i]);
			}
			else
			{
				particles.ParticlePosition(i, _spiralInterpolations[i]);
			}
		}
	}

	private void GenerateSpiralInterpolations()
	{
		_startPos = start.position;
		_endPos = end.position;
		_lookDirection = (_endPos - _startPos).normalized;
		_r = radius;
		_i = swirlIterations;
		_spiralInterpolations = new Vector3[particles.particleCount];
		float num = Vector3.Distance(_startPos, _endPos);
		float num2 = (float)Math.PI;
		for (int i = 0; i < _spiralInterpolations.Length; i++)
		{
			float num3 = (float)i * 1f / (float)particles.particleCount;
			float num4 = radiusTimeScale.Evaluate(num3);
			Vector3 vector = new Vector3(_r * num4 * Mathf.Sin(_i * num3 * num2 * 2f), _r * num4 * Mathf.Cos(_i * num3 * num2 * 2f), num3 * num);
			Vector3 vector2 = _startPos + Quaternion.LookRotation(_lookDirection) * vector;
			_spiralInterpolations[i] = vector2;
		}
	}
}
