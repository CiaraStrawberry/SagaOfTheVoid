using System.Collections.Generic;

namespace TrueSync.Physics3D;

public abstract class Multishape : Shape
{
	internal bool isClone;

	private Stack<Multishape> workingCloneStack = new Stack<Multishape>();

	public bool IsClone => isClone;

	public abstract void SetCurrentShape(int index);

	public abstract int Prepare(ref TSBBox box);

	public abstract int Prepare(ref TSVector rayOrigin, ref TSVector rayDelta);

	protected abstract Multishape CreateWorkingClone();

	public Multishape RequestWorkingClone()
	{
		lock (workingCloneStack)
		{
			Multishape multishape;
			if (workingCloneStack.Count == 0)
			{
				multishape = CreateWorkingClone();
				multishape.workingCloneStack = workingCloneStack;
				workingCloneStack.Push(multishape);
			}
			multishape = workingCloneStack.Pop();
			multishape.isClone = true;
			return multishape;
		}
	}

	public override void UpdateShape()
	{
		lock (workingCloneStack)
		{
			workingCloneStack.Clear();
		}
		base.UpdateShape();
	}

	public void ReturnWorkingClone()
	{
		lock (workingCloneStack)
		{
			workingCloneStack.Push(this);
		}
	}

	public override void GetBoundingBox(ref TSMatrix orientation, out TSBBox box)
	{
		TSBBox box2 = TSBBox.LargeBox;
		int num = Prepare(ref box2);
		box = TSBBox.SmallBox;
		for (int i = 0; i < num; i++)
		{
			SetCurrentShape(i);
			base.GetBoundingBox(ref orientation, out box2);
			TSBBox.CreateMerged(ref box, ref box2, out box);
		}
	}

	public override void MakeHull(ref List<TSVector> triangleList, int generationThreshold)
	{
	}

	public override void CalculateMassInertia()
	{
		geomCen = TSVector.zero;
		inertia = TSMatrix.Identity;
		TSVector.Subtract(ref boundingBox.max, ref boundingBox.min, out var result);
		mass = result.x * result.y * result.z;
		inertia.M11 = FP.One / (12 * FP.One) * mass * (result.y * result.y + result.z * result.z);
		inertia.M22 = FP.One / (12 * FP.One) * mass * (result.x * result.x + result.z * result.z);
		inertia.M33 = FP.One / (12 * FP.One) * mass * (result.x * result.x + result.y * result.y);
	}
}
