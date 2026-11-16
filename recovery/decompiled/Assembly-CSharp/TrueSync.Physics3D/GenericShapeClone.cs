namespace TrueSync.Physics3D;

public class GenericShapeClone
{
	public TSMatrix inertia;

	public FP mass;

	public TSBBox boundingBox;

	public TSVector geomCen;

	public TSVector vector1;

	public TSVector vector2;

	public FP fp1;

	public FP fp2;

	public FP fp3;

	public void Clone(Shape sh)
	{
		inertia = sh.inertia;
		mass = sh.mass;
		boundingBox = sh.boundingBox;
		geomCen = sh.geomCen;
		if (sh is BoxShape)
		{
			CloneBox((BoxShape)sh);
		}
		else if (sh is SphereShape)
		{
			CloneSphere((SphereShape)sh);
		}
		else if (sh is ConeShape)
		{
			CloneCone((ConeShape)sh);
		}
		else if (sh is CylinderShape)
		{
			CloneCylinder((CylinderShape)sh);
		}
		else if (sh is CapsuleShape)
		{
			CloneCapsule((CapsuleShape)sh);
		}
	}

	private void CloneBox(BoxShape sh)
	{
		vector1 = sh.size;
		vector2 = sh.halfSize;
	}

	private void CloneSphere(SphereShape sh)
	{
		fp1 = sh.radius;
	}

	private void CloneCone(ConeShape sh)
	{
		fp1 = sh.radius;
		fp2 = sh.height;
		fp3 = sh.sina;
	}

	private void CloneCylinder(CylinderShape sh)
	{
		fp1 = sh.radius;
		fp2 = sh.height;
	}

	private void CloneCapsule(CapsuleShape sh)
	{
		fp1 = sh.radius;
		fp2 = sh.length;
	}

	public void Restore(Shape sh)
	{
		sh.inertia = inertia;
		sh.mass = mass;
		sh.boundingBox = boundingBox;
		sh.geomCen = geomCen;
		if (sh is BoxShape)
		{
			RestoreBox((BoxShape)sh);
		}
		else if (sh is SphereShape)
		{
			RestoreSphere((SphereShape)sh);
		}
		else if (sh is ConeShape)
		{
			RestoreCone((ConeShape)sh);
		}
		else if (sh is CylinderShape)
		{
			RestoreCylinder((CylinderShape)sh);
		}
		else if (sh is CapsuleShape)
		{
			RestoreCapsule((CapsuleShape)sh);
		}
	}

	private void RestoreBox(BoxShape sh)
	{
		sh.size = vector1;
		sh.halfSize = vector2;
	}

	private void RestoreSphere(SphereShape sh)
	{
		sh.radius = fp1;
	}

	private void RestoreCone(ConeShape sh)
	{
		sh.radius = fp1;
		sh.height = fp2;
		sh.sina = fp3;
	}

	private void RestoreCylinder(CylinderShape sh)
	{
		sh.radius = fp1;
		sh.height = fp2;
	}

	private void RestoreCapsule(CapsuleShape sh)
	{
		sh.radius = fp1;
		sh.length = fp2;
	}
}
