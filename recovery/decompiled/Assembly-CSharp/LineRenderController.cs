using TrueSync;
using UnityEngine;

public class LineRenderController : TrueSyncBehaviour
{
	private LineRenderer linerender;

	public Vector3[] Positions;

	private GameObject WorldParent;

	public int size = 36;

	public float timebetween = 0.06f;

	private GameObject WorldBase;

	private Vector3 lastscale;

	public bool started;

	public bool missile;

	public Vector3 lastposition;

	public Vector3[] positionsrealtive;

	public CustomPathfinding parent;

	public GameObject linetargetend;

	private int i;

	private int skip;

	private int skipthisrun;

	private bool lastskip;

	public void Start()
	{
		if (missile)
		{
			LineRenderer component = gameObject.GetComponent<LineRenderer>();
			component.material = new Material(Shader.Find("Particles/Additive"));
			component.startColor = Color.blue;
			component.endColor = Color.white;
		}
		this.i = 0;
		if (!missile)
		{
			parent = transform.parent.parent.GetComponent<CustomPathfinding>();
		}
		Positions = new Vector3[size];
		positionsrealtive = new Vector3[size];
		transform.localPosition += new Vector3(0f, 0f, -5f);
		linerender = GetComponent<LineRenderer>();
		WorldParent = GameObject.Find("Objects");
		WorldBase = GameObject.Find("WorldScaleBase");
		linerender.startWidth = 1f;
		linerender.endWidth = 0f;
		linerender.positionCount = Positions.Length;
		for (int i = 0; i < Positions.Length; i++)
		{
			ref Vector3 reference = ref Positions[i];
			reference = WorldParent.transform.InverseTransformPoint(transform.position);
		}
		for (int j = 0; j < Positions.Length; j++)
		{
			linerender.SetPosition(j, WorldParent.transform.TransformPoint(Positions[j]));
		}
		linerender.startWidth = WorldBase.transform.localScale.x / 38000f;
		Rep();
	}

	private void Update()
	{
		if (i == 0 && missile)
		{
			Start();
		}
		i++;
		if (missile)
		{
			OnSyncedUpdate();
		}
		if (!started)
		{
			linerender.startWidth = WorldBase.transform.localScale.x / 35000f;
			if (missile)
			{
				linerender.startWidth *= 0.25f;
			}
		}
	}

	public override void OnSyncedUpdate()
	{
		if (missile)
		{
			OnSyncedUpdatspec();
		}
	}

	private void skipthisrunadd()
	{
		if (skipthisrun < 5)
		{
			skipthisrun++;
		}
	}

	private void skipthisrunremove()
	{
		if (skipthisrun > 0)
		{
			lastskip = true;
			skipthisrun--;
		}
	}

	public void OnSyncedUpdatspec()
	{
		if (parent == null || parent.timepassedsincespawn > 180f)
		{
			started = true;
			if (this != null && linerender != null)
			{
				if (WorldBase == null)
				{
					WorldBase = GameObject.Find("WorldScaleBase");
				}
				if (WorldBase.transform.localScale != lastscale)
				{
					skipthisrunadd();
				}
				else
				{
					skipthisrunremove();
				}
				if (skipthisrun != 0)
				{
					lastscale = WorldBase.transform.localScale;
					for (int i = 0; i < Positions.Length; i++)
					{
						linerender.SetPosition(i, WorldParent.transform.TransformPoint(WorldParent.transform.InverseTransformPoint(transform.position) + positionsrealtive[i]));
					}
					if (missile)
					{
						linerender.startWidth = WorldBase.transform.localScale.x / 12000f * 0.1f;
					}
					else
					{
						linerender.startWidth = WorldBase.transform.localScale.x / 12000f;
					}
					for (int j = 0; j < Positions.Length; j++)
					{
						ref Vector3 reference = ref Positions[j];
						reference = WorldParent.transform.InverseTransformPoint(transform.position) + positionsrealtive[j];
					}
				}
				else
				{
					linerender.SetPosition(0, transform.position);
					ref Vector3 reference2 = ref positionsrealtive[0];
					reference2 = new Vector3(0f, 0f, 0f);
					for (int num = positionsrealtive.Length - 1; num > 0; num--)
					{
						ref Vector3 reference3 = ref positionsrealtive[num];
						reference3 = Positions[num] - WorldParent.transform.InverseTransformPoint(transform.position);
					}
					skip++;
					if (skip == 3)
					{
						lastskip = false;
						skip = 0;
						Rep();
					}
				}
				lastposition = transform.position;
			}
			else if (linerender == null && this != null)
			{
				Start();
			}
		}
		else
		{
			if (missile)
			{
				return;
			}
			if (linerender == null && this != null)
			{
				Start();
				return;
			}
			for (int k = 0; k < size; k++)
			{
				linerender.SetPosition(k, Vector3.Lerp(transform.position, linetargetend.transform.position, (float)k / (float)size));
			}
		}
	}

	private void Rep()
	{
		if (!(this != null))
		{
			return;
		}
		if (WorldBase == null)
		{
			WorldBase = GameObject.Find("WorldScaleBase");
		}
		for (int num = Positions.Length - 1; num > 0; num--)
		{
			ref Vector3 reference = ref Positions[num];
			reference = Positions[num - 1];
		}
		ref Vector3 reference2 = ref Positions[0];
		reference2 = WorldParent.transform.InverseTransformPoint(transform.position);
		for (int num2 = Positions.Length - 1; num2 > 0; num2--)
		{
			if (Positions[num2].x == 0f)
			{
				ref Vector3 reference3 = ref Positions[num2];
				reference3 = Positions[0];
			}
		}
		for (int i = 0; i < Positions.Length; i++)
		{
			if ((i > 0 && Positions[i] != Positions[i - 1]) || Positions[i] == WorldParent.transform.InverseTransformPoint(transform.position))
			{
				linerender.SetPosition(i, WorldParent.transform.TransformPoint(Positions[i]));
			}
		}
		linerender.SetPosition(0, WorldParent.transform.TransformPoint(Positions[0]));
	}
}
