namespace TrueSync.Physics2D;

public class GenericShapeClone2D
{
	private FP _density;

	private FP _radius;

	private FP _2radius;

	private MassData massData;

	private TSVector2 _position;

	private Vertices _vertices;

	private Vertices _normals;

	public void Clone(Shape sh)
	{
		_density = sh._density;
		_radius = sh._radius;
		_2radius = sh._2radius;
		massData = sh.MassData;
		if (sh is PolygonShape)
		{
			ClonePolygon((PolygonShape)sh);
		}
		else if (sh is CircleShape)
		{
			CloneCircle((CircleShape)sh);
		}
	}

	private void ClonePolygon(PolygonShape sh)
	{
		if (_vertices == null)
		{
			_vertices = new Vertices();
		}
		else
		{
			_vertices.Clear();
		}
		if (_normals == null)
		{
			_normals = new Vertices();
		}
		else
		{
			_normals.Clear();
		}
		_vertices.AddRange(sh._vertices);
		_normals.AddRange(sh._normals);
	}

	private void CloneCircle(CircleShape sh)
	{
		_position = sh._position;
	}

	public void Restore(Shape sh)
	{
		sh._density = _density;
		sh._radius = _radius;
		sh._2radius = _2radius;
		sh.MassData = massData;
		if (sh is PolygonShape)
		{
			RestorePolygon((PolygonShape)sh);
		}
		else if (sh is CircleShape)
		{
			RestoreCircle((CircleShape)sh);
		}
	}

	private void RestorePolygon(PolygonShape sh)
	{
		sh._vertices.Clear();
		sh._vertices.AddRange(_vertices);
		sh._normals.Clear();
		sh._normals.AddRange(_normals);
	}

	private void RestoreCircle(CircleShape sh)
	{
		sh._position = _position;
	}
}
