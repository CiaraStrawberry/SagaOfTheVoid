using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PaintObjectC
{
	[HideInInspector]
	public List<PaintPositionC> paintPositions;

	[HideInInspector]
	public int positionLength;

	[HideInInspector]
	public Vector3 lastPaintPosition;

	[HideInInspector]
	public COLLISIONTYPEC collisionType;

	public float minDepth = -1f;

	public float maxDepth = 1f;

	public float spacing;

	public LayerMask layerMask = -1;

	public PlaygroundBrushC brush;

	public bool exceedMaxStopsPaint;

	public bool initialized;

	public void Initialize()
	{
		paintPositions = new List<PaintPositionC>();
		brush = new PlaygroundBrushC();
		lastPaintPosition = PlaygroundC.initialTargetPosition;
		initialized = true;
	}

	public bool Paint(Ray ray, Color32 color)
	{
		if (collisionType == COLLISIONTYPEC.Physics3D)
		{
			if (Physics.Raycast(ray, out var hitInfo, brush.distance, layerMask))
			{
				Paint(hitInfo.point, hitInfo.normal, hitInfo.transform, color);
				lastPaintPosition = hitInfo.point;
				return true;
			}
		}
		else
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, brush.distance, layerMask, minDepth, maxDepth);
			if (raycastHit2D.collider != null)
			{
				Paint(raycastHit2D.point, raycastHit2D.normal, raycastHit2D.transform, color);
				lastPaintPosition = raycastHit2D.point;
				return true;
			}
		}
		return false;
	}

	public int Paint(Vector3 pos, Vector3 norm, Transform parent, Color32 color)
	{
		PaintPositionC paintPositionC = new PaintPositionC();
		if ((bool)parent)
		{
			paintPositionC.parent = parent;
			paintPositionC.initialPosition = parent.InverseTransformPoint(pos);
			paintPositionC.initialNormal = parent.InverseTransformDirection(norm);
		}
		else
		{
			paintPositionC.initialPosition = pos;
			paintPositionC.initialNormal = norm;
		}
		paintPositionC.color = color;
		paintPositionC.position = paintPositionC.initialPosition;
		paintPositions.Add(paintPositionC);
		positionLength = paintPositions.Count;
		if (!exceedMaxStopsPaint && positionLength > PlaygroundC.reference.paintMaxPositions)
		{
			paintPositions.RemoveAt(0);
			positionLength = paintPositions.Count;
		}
		return positionLength - 1;
	}

	public bool Erase(Vector3 pos, float radius)
	{
		bool result = false;
		if (paintPositions.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < paintPositions.Count; i++)
		{
			if (Vector3.Distance(paintPositions[i].position, pos) < radius)
			{
				paintPositions.RemoveAt(i);
				result = true;
			}
		}
		positionLength = paintPositions.Count;
		return result;
	}

	public bool Erase(int index)
	{
		if (index >= 0 && index < positionLength)
		{
			paintPositions.RemoveAt(index);
			positionLength = paintPositions.Count;
			return true;
		}
		return false;
	}

	public Vector3 GetPosition(int index)
	{
		index %= positionLength;
		return paintPositions[index].position;
	}

	public Color32 GetColor(int index)
	{
		index %= positionLength;
		return paintPositions[index].color;
	}

	public Vector3 GetNormal(int index)
	{
		index %= positionLength;
		return paintPositions[index].initialNormal;
	}

	public Transform GetParent(int index)
	{
		index %= positionLength;
		return paintPositions[index].parent;
	}

	public Quaternion GetRotation(int index)
	{
		index %= positionLength;
		return paintPositions[index].parentRotation;
	}

	public void Update()
	{
		for (int i = 0; i < paintPositions.Count; i++)
		{
			if (!(paintPositions[i].parent == null))
			{
				Update(i);
			}
		}
	}

	public void Update(int thisPosition)
	{
		thisPosition %= positionLength;
		paintPositions[thisPosition].position = paintPositions[thisPosition].parent.TransformPoint(paintPositions[thisPosition].initialPosition);
		paintPositions[thisPosition].parentRotation = paintPositions[thisPosition].parent.rotation;
	}

	public void RemoveNonParented()
	{
		for (int i = 0; i < paintPositions.Count; i++)
		{
			if (paintPositions[i].parent == null)
			{
				paintPositions.RemoveAt(i);
				i--;
			}
		}
	}

	public bool RemoveNonParented(int thisPosition)
	{
		thisPosition %= positionLength;
		if (paintPositions[thisPosition].parent == null)
		{
			paintPositions.RemoveAt(thisPosition);
			return true;
		}
		return false;
	}

	public void ClearPaint()
	{
		paintPositions = new List<PaintPositionC>();
		positionLength = 0;
	}

	public PaintObjectC Clone()
	{
		PaintObjectC paintObjectC = new PaintObjectC();
		if (paintPositions != null && paintPositions.Count > 0)
		{
			paintObjectC.paintPositions = new List<PaintPositionC>();
			paintObjectC.paintPositions.AddRange(paintPositions);
		}
		paintObjectC.positionLength = positionLength;
		paintObjectC.lastPaintPosition = lastPaintPosition;
		paintObjectC.spacing = spacing;
		paintObjectC.layerMask = layerMask;
		paintObjectC.collisionType = collisionType;
		if (brush != null)
		{
			paintObjectC.brush = brush.Clone();
		}
		else
		{
			paintObjectC.brush = new PlaygroundBrushC();
		}
		paintObjectC.exceedMaxStopsPaint = exceedMaxStopsPaint;
		paintObjectC.initialized = initialized;
		return paintObjectC;
	}
}
