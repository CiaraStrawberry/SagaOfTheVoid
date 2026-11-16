using System.Collections.Generic;
using UnityEngine;

namespace PlaygroundSplines;

[ExecuteInEditMode]
public class PlaygroundSpline : MonoBehaviour
{
	[SerializeField]
	private List<Vector3> points = new List<Vector3>();

	[SerializeField]
	private List<BezierControlPointMode> modes = new List<BezierControlPointMode>();

	[SerializeField]
	private bool loop;

	[HideInInspector]
	public List<TransformNode> transformNodes = new List<TransformNode>();

	[HideInInspector]
	public bool reverse;

	[HideInInspector]
	public float timeOffset;

	[HideInInspector]
	public Vector3 positionOffset;

	[HideInInspector]
	public Transform splineTransform;

	[HideInInspector]
	public Matrix4x4 splineTransformMx;

	[HideInInspector]
	public List<Transform> usedBy = new List<Transform>();

	[HideInInspector]
	public float fixedVelocityOnNewNode = 0.5f;

	[HideInInspector]
	public bool moveTransformsAsBeziers;

	[HideInInspector]
	public bool exportWithNodeStructure;

	public static bool drawSplinePreviews = true;

	[HideInInspector]
	public bool drawGizmo = true;

	[HideInInspector]
	public float bezierWidth = 2f;

	private Vector3 previousPosition;

	private Quaternion previousRotation;

	private Vector3 previousScale;

	private bool isReady;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
			if (value && NodeCount > 1)
			{
				modes[modes.Count - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount => points.Count;

	public int NodeCount => (points.Count - 1) / 3;

	public bool IsReady()
	{
		return isReady;
	}

	public bool AddUser(Transform thisTransform)
	{
		if (!usedBy.Contains(thisTransform))
		{
			usedBy.Add(thisTransform);
			return true;
		}
		return false;
	}

	public bool RemoveUser(Transform thisTransform)
	{
		if (usedBy.Contains(thisTransform))
		{
			usedBy.Remove(thisTransform);
			return true;
		}
		return false;
	}

	public bool HasUser(Transform thisTransform)
	{
		return usedBy.Contains(thisTransform);
	}

	public Vector3 GetControlPoint(int index)
	{
		return GetPoint(index);
	}

	public void SetControlPoint(int index, Vector3 point, Vector3 offset)
	{
		SetControlPoint(index, point - offset);
	}

	public void SetControlPoint(int index, Vector3 point)
	{
		if (index < 0)
		{
			index = 0;
		}
		if (index % 3 == 0)
		{
			Vector3 vector = point - GetPoint(index);
			if (loop)
			{
				if (index == 0)
				{
					Vector3 point2 = GetPoint(1);
					SetPoint(1, point2 + vector);
					point2 = GetPoint(points.Count - 2);
					SetPoint(points.Count - 2, point2 + vector);
					if (moveTransformsAsBeziers || !PointHasTransform(points.Count - 1))
					{
						SetPoint(points.Count - 1, point);
					}
				}
				else if (index == points.Count - 1)
				{
					SetPoint(0, point);
					Vector3 point2 = GetPoint(1);
					SetPoint(1, point2 + vector);
					point2 = GetPoint(index - 1);
					SetPoint(index - 1, point2 + vector);
				}
				else
				{
					Vector3 point2 = GetPoint(index - 1);
					SetPoint(index - 1, point2 + vector);
					point2 = GetPoint(index + 1);
					SetPoint(index + 1, point2 + vector);
				}
			}
			else
			{
				if (index > 0 && (moveTransformsAsBeziers || !PointHasTransform(index - 1)))
				{
					Vector3 point2 = GetPoint(index - 1);
					SetPoint(index - 1, point2 + vector);
				}
				if (index + 1 < points.Count && (moveTransformsAsBeziers || !PointHasTransform(index + 1)))
				{
					Vector3 point2 = GetPoint(index + 1);
					SetPoint(index + 1, point2 + vector);
				}
			}
		}
		SetPoint(index, point);
		EnforceMode(index);
	}

	public void SetPoints(Vector3[] vectors)
	{
		if (vectors.Length != points.Count)
		{
			Debug.Log("Please ensure the same length of your passed in vectors (" + vectors.Length + ") as the current points (" + points.Count + "). Use PlaygroundSpline.ControlPointCount to get the current count.");
		}
		else
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i] = vectors[i];
			}
		}
	}

	public bool PointHasTransform(int index)
	{
		return transformNodes[index].IsAvailable();
	}

	public void TranslateSpline(Vector3 translation)
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i] += translation;
		}
	}

	public Vector3 GetTransformPosition()
	{
		return previousPosition;
	}

	public Quaternion GetTransformRotation()
	{
		return previousRotation;
	}

	public Vector3 GetTransformScale()
	{
		return previousScale;
	}

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		modes[num] = mode;
		if (loop)
		{
			if (num == 0)
			{
				modes[modes.Count - 1] = mode;
			}
			else if (num == modes.Count - 1)
			{
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!loop && (num == 0 || num == modes.Count - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = points.Count - 2;
			}
			num4 = num2 + 1;
			if (num4 >= points.Count)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= points.Count)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = points.Count - 2;
			}
		}
		Vector3 point = GetPoint(num2);
		Vector3 vector = point - GetPoint(num3);
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			vector = vector.normalized * Vector3.Distance(point, GetPoint(num4));
		}
		if (moveTransformsAsBeziers || !PointHasTransform(num4))
		{
			SetPoint(num4, point + vector);
		}
	}

	public Vector3 GetPoint(float t)
	{
		if (reverse)
		{
			t = 1f - t;
			t = (t - timeOffset) % 1f;
			if (t < 0f)
			{
				t = 1f + t;
			}
		}
		else
		{
			t = (t + timeOffset) % 1f;
		}
		int num;
		if (t >= 1f)
		{
			num = points.Count - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)NodeCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return splineTransformMx.MultiplyPoint3x4(Bezier.GetPoint(GetInversePoint(num), GetInversePoint(num + 1), GetInversePoint(num + 2), GetInversePoint(num + 3), t) + positionOffset);
	}

	public Vector3 GetVelocity(float t)
	{
		if (reverse)
		{
			t = 1f - t;
		}
		t = (t + timeOffset) % 1f;
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Count - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)NodeCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return splineTransformMx.MultiplyPoint3x4(Bezier.GetFirstDerivative(GetInversePoint(num), GetInversePoint(num + 1), GetInversePoint(num + 2), GetInversePoint(num + 3), t) + positionOffset) - previousPosition;
	}

	public Vector3 GetPoint(int index)
	{
		if (transformNodes[index].IsAvailable())
		{
			return transformNodes[index].GetPosition();
		}
		return points[index];
	}

	public Vector3 GetInversePoint(int index)
	{
		if (transformNodes[index].IsAvailable())
		{
			return transformNodes[index].GetInvsersePosition();
		}
		return points[index];
	}

	public Vector3 GetPointWorldSpace(int index)
	{
		if (transformNodes[index].IsAvailable())
		{
			return transformNodes[index].GetPosition();
		}
		return splineTransformMx.MultiplyPoint3x4(points[index] + positionOffset);
	}

	private void SetPoint(int index, Vector3 position)
	{
		if (transformNodes[index].IsAvailable())
		{
			transformNodes[index].SetPosition(position);
		}
		else
		{
			points[index] = position;
		}
	}

	private void TranslatePoint(int index, Vector3 translation)
	{
		if (transformNodes[index].IsAvailable())
		{
			transformNodes[index].Translate(translation);
		}
		else
		{
			points[index] += translation;
		}
	}

	private float CPOB(Vector3 aP, float aStart, float aEnd, int aSteps)
	{
		aStart = Mathf.Clamp01(aStart);
		aEnd = Mathf.Clamp01(aEnd);
		float num = (aEnd - aStart) / (float)aSteps;
		float result = 0f;
		float num2 = float.MaxValue;
		for (int i = 0; i < aSteps; i++)
		{
			float num3 = aStart + num * (float)i;
			float sqrMagnitude = (GetPoint(num3) - aP).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				num2 = sqrMagnitude;
				result = num3;
			}
		}
		return result;
	}

	public float ClosestTimeFromPoint(Vector3 aP)
	{
		float num = CPOB(aP, 0f, 1f, 10);
		float num2 = 0.1f;
		for (int i = 0; i < 4; i++)
		{
			num = CPOB(aP, num - num2, num + num2, 10);
			num2 /= 9f;
		}
		return num;
	}

	public Vector3 ClosestPointFromPosition(Vector3 aP)
	{
		return GetPoint(ClosestTimeFromPoint(aP));
	}

	public Vector3 GetDirection(float t)
	{
		return (GetPoint(t + 0.001f) - GetPoint(t)).normalized;
	}

	public void AddNode()
	{
		AddNode((points.Count - 1) / 3);
	}

	public void AddNode(int index)
	{
		int num = index * 3;
		Vector3 point = GetPoint(num);
		Vector3 vector = ((index <= 0) ? (GetPoint(num + 1) - GetPoint(num)) : (GetPoint(num) - GetPoint(num - 1)));
		vector *= fixedVelocityOnNewNode;
		points.InsertRange(num + 1, new Vector3[3]);
		point += vector;
		points[num + 2] = point;
		point += vector;
		points[num + 1] = point;
		point += vector;
		points[num + 3] = point;
		transformNodes.InsertRange(num + 1, new TransformNode[3]
		{
			new TransformNode(),
			new TransformNode(),
			new TransformNode()
		});
		BezierControlPointMode value = modes[index];
		modes.Insert(index, BezierControlPointMode.Aligned);
		modes[index] = value;
		EnforceMode(index);
		SetControlPoint((index + 1) * 3, GetPoint((index + 1) * 3));
		if (loop)
		{
			points[points.Count - 1] = points[0];
			modes[modes.Count - 1] = modes[0];
			EnforceMode(0);
		}
	}

	public void RemoveFirst()
	{
		RemoveNode(0);
	}

	public void RemoveLast()
	{
		RemoveNode((points.Count - 1) / 3);
	}

	public void RemoveNode(int index)
	{
		index = Mathf.Clamp(index, 0, points.Count - 1);
		int num = index * 3;
		if (points.Count > 4)
		{
			if (num < points.Count - 1)
			{
				points.RemoveRange(num, 3);
				transformNodes.RemoveRange(num, 3);
			}
			else
			{
				points.RemoveRange(num - 2, 3);
				transformNodes.RemoveRange(num - 2, 3);
			}
			modes.RemoveAt(index);
			EnforceMode(index - 1);
			if (index > 0)
			{
				SetControlPoint((index - 1) * 3, GetPoint((index - 1) * 3));
			}
			else
			{
				SetControlPoint(0, GetPoint(0));
			}
		}
	}

	public void ReverseAllNodes()
	{
		points.Reverse();
		transformNodes.Reverse();
		modes.Reverse();
	}

	public void SwapNodes(int from, int to)
	{
		from = Mathf.Clamp(from, 0, NodeCount + 1);
		to = Mathf.Clamp(to, 0, NodeCount + 1);
		if (from == to)
		{
			Debug.Log("From and to is expecting different node indexes.");
			return;
		}
		int index = from * 3;
		int index2 = to * 3;
		Vector3 point = points[index];
		Vector3 point2 = points[index2];
		TransformNode value = transformNodes[index];
		TransformNode value2 = transformNodes[index2];
		BezierControlPointMode value3 = modes[from];
		BezierControlPointMode value4 = modes[to];
		SetControlPoint(index, point2);
		SetControlPoint(index2, point);
		transformNodes[index] = value2;
		transformNodes[index2] = value;
		modes[from] = value4;
		modes[to] = value3;
	}

	public Transform[] ExportToTransforms()
	{
		Transform[] array = new Transform[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			int num = (i + 1) / 3;
			int num2 = ((i >= 3) ? (i % 3 % 2) : 0);
			bool flag = i == 0 || i % 3 == 0;
			array[i] = new GameObject((!flag) ? ("Node " + num + " - Bezier " + num2) : ("Node " + num)).transform;
			array[i].parent = splineTransform;
			array[i].position = splineTransform.TransformPoint(GetInversePoint(i) + positionOffset);
		}
		if (exportWithNodeStructure)
		{
			for (int j = 2; j < array.Length; j++)
			{
				int num3 = ((j >= 3) ? (j % 3 % 2) : 0);
				if (j != 0 && j % 3 != 0)
				{
					array[j].parent = array[(num3 != 0) ? (j - 1) : (j + 1)];
				}
			}
			array[1].parent = array[0];
		}
		return array;
	}

	public Vector3[] ExportToVector3()
	{
		Vector3[] array = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = GetPoint(i) + positionOffset;
		}
		return array;
	}

	public void Reset()
	{
		points = new List<Vector3>
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new List<BezierControlPointMode>
		{
			BezierControlPointMode.Aligned,
			BezierControlPointMode.Aligned
		};
		transformNodes = new List<TransformNode>
		{
			new TransformNode(),
			new TransformNode(),
			new TransformNode(),
			new TransformNode()
		};
	}

	private void OnEnable()
	{
		isReady = false;
		splineTransform = transform;
		SetMatrix();
	}

	private void Update()
	{
		SetMatrix();
		for (int i = 0; i < transformNodes.Count; i++)
		{
			transformNodes[i].Update(splineTransform);
		}
	}

	private void SetMatrix()
	{
		if (previousPosition != splineTransform.position || previousRotation != splineTransform.rotation || previousScale != splineTransform.localScale)
		{
			splineTransformMx.SetTRS(splineTransform.position, splineTransform.rotation, splineTransform.localScale);
		}
		previousPosition = splineTransform.position;
		previousRotation = splineTransform.rotation;
		previousScale = splineTransform.localScale;
		isReady = true;
	}
}
