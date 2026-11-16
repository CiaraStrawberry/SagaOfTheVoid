using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class Perimeter
{
	internal float edge { get; private set; }

	internal float edgeMid { get; private set; }

	internal Vector3 edgeVector { get; private set; }

	internal Vector3 insideCornerOne { get; private set; }

	internal Vector3 insideCornerTwo { get; private set; }

	internal Vector3 outsideCornerOne { get; private set; }

	internal Vector3 outsideCornerTwo { get; private set; }

	internal Perimeter perpendicularOne { get; private set; }

	internal Perimeter perpendicularTwo { get; private set; }

	internal Perimeter(Vector3 edgeVector, float edge, float edgeMid)
	{
		this.edgeVector = edgeVector;
		this.edge = edge;
		this.edgeMid = edgeMid;
	}

	internal void SetPerpendiculars(Perimeter perpOne, Perimeter perpTwo, float step)
	{
		perpendicularOne = perpOne;
		perpendicularTwo = perpTwo;
		insideCornerOne = GetPoint(perpOne.edgeMid);
		insideCornerTwo = GetPoint(perpTwo.edgeMid);
		outsideCornerOne = insideCornerOne + (edgeVector + perpOne.edgeVector) * step;
		outsideCornerTwo = insideCornerTwo + (edgeVector + perpTwo.edgeVector) * step;
	}

	internal Vector3 GetPoint(Vector3 refPoint)
	{
		if (edgeVector.x != 0f)
		{
			return new Vector3(edgeMid, 0f, refPoint.z);
		}
		return new Vector3(refPoint.x, 0f, edgeMid);
	}

	private Vector3 GetPoint(float otherAxis)
	{
		if (edgeVector.x != 0f)
		{
			return new Vector3(edgeMid, 0f, otherAxis);
		}
		return new Vector3(otherAxis, 0f, edgeMid);
	}
}
