using Forge3D;
using UnityEngine;

public class ProjectileMovementCom : MonoBehaviour
{
	public GameObject targetgam;

	public GameObject Explosion;

	public Vector3 startpos;

	public Vector3 origin;

	public float dist1;

	public float dist2;

	public int damage;

	public float turnspeed = 90f;

	public float timepassed;

	public bool ishit;

	public bool ismissile;

	public bool islazer;

	public GameObject start;

	public GameObject end;

	public LineRenderer ren;

	private GameObject World;

	private GameObject objpos;

	public string debug;

	public int framesin;

	public bool started;

	public float speed = 16f;

	public Transform impact;

	public Material missilematerail;

	private Vector3 lastendpos;

	public float linerenmodifier;

	public _Weapon.guntype guntype;

	public Vector3 mispos;

	public GameObject SourceGun;

	public void Start()
	{
		if (GetComponent<LineRenderer>() != null && !islazer)
		{
			GetComponent<LineRenderer>().material = missilematerail;
		}
		turnspeed = 130f;
		if (ismissile)
		{
			turnspeed = 220f;
		}
		objpos = GameObject.Find("Objects");
		timepassed = 0f;
		World = GameObject.Find("WorldScaleBase");
		framesin = 0;
		started = true;
		if (islazer && (bool)ren && start.gameObject != null && end.gameObject != null)
		{
			ren.SetPosition(0, SourceGun.transform.position);
			ren.SetPosition(1, end.transform.position);
			ren.startWidth = 1.5E-05f * World.transform.localScale.x;
			ren.endWidth = 1.5E-05f * World.transform.localScale.x;
			ren.transform.parent = GameObject.Find("Working").transform;
		}
		if (transform.childCount > 0)
		{
			impact = transform.GetChild(0);
		}
		if (islazer)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if ((bool)ren && ren.material == null)
		{
			Destroythisgam();
		}
	}

	public void endgam()
	{
		started = false;
	}

	private void Update()
	{
		if (timepassed > 1f && ishit)
		{
			turnspeed *= 1.1f;
		}
		if (islazer && SourceGun == null)
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
		}
		if (islazer)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (framesin < 1)
		{
			startpos = objpos.transform.InverseTransformPoint(transform.position);
		}
		framesin++;
		if (framesin == 2 && islazer && !ishit && targetgam != null)
		{
			mispos = targetgam.transform.localPosition + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
		}
		timepassed += Time.deltaTime;
		if (ismissile && timepassed < 2f)
		{
			speed = 6f;
		}
		else
		{
			speed = 22f;
		}
		if (ismissile && timepassed > 2f)
		{
			speed = 14f;
		}
		if (guntype == _Weapon.guntype.CapitalMainGun)
		{
			speed = 40f;
		}
		if (started)
		{
			if (!islazer)
			{
				if ((bool)targetgam && targetgam.gameObject != null)
				{
					Vector3 forward = targetgam.transform.position - transform.position;
					Quaternion to = Quaternion.LookRotation(forward);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, to, turnspeed * Time.deltaTime);
				}
				else
				{
					ishit = false;
				}
				transform.localPosition += transform.forward * speed;
				if (ishit)
				{
					Vector3 forward2 = targetgam.transform.position - transform.position;
					Quaternion to2 = Quaternion.LookRotation(forward2);
					if (!ismissile)
					{
						transform.rotation = Quaternion.RotateTowards(transform.rotation, to2, turnspeed * Time.deltaTime);
					}
					if (Vector3.Distance(objpos.transform.InverseTransformPoint(transform.position), objpos.transform.InverseTransformPoint(targetgam.transform.position)) < 40f || (timepassed > 5f && !ismissile))
					{
						Destroythisgam();
					}
				}
				else if ((bool)targetgam && targetgam.gameObject != null)
				{
					if (timepassed > 7f || ((bool)targetgam && targetgam.gameObject == null) || (Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(transform.position)) > Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(targetgam.transform.position)) && !ismissile) || (ismissile && (double)Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(transform.position)) > (double)Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(targetgam.transform.position)) * 1.4))
					{
						Destroythisgam();
					}
				}
				else if (timepassed > 3f)
				{
					Destroythisgam();
				}
			}
			if (islazer && (bool)ren && start.gameObject != null)
			{
				if (end.gameObject != null)
				{
					lastendpos = end.transform.position;
				}
				ren.SetPosition(0, SourceGun.transform.position);
				if (ishit && end.gameObject != null)
				{
					ren.SetPosition(1, end.transform.position);
				}
				else if (mispos != new Vector3(0f, 0f, 0f))
				{
					ren.SetPosition(1, transform.parent.TransformPoint(mispos));
				}
				else
				{
					ren.SetPosition(1, SourceGun.transform.position);
				}
				if (start == null && end == null)
				{
					ren.SetPosition(0, new Vector3(0f, 0f, 0f));
					ren.SetPosition(1, new Vector3(0f, 0f, 0f));
				}
				if (guntype == _Weapon.guntype.Lazer)
				{
					if (!impact.gameObject.GetActive())
					{
						impact.gameObject.SetActive(value: true);
					}
					if (ishit && end.gameObject != null)
					{
						impact.transform.position = end.transform.position;
					}
					else
					{
						impact.transform.position = transform.parent.TransformPoint(mispos);
					}
					impact.transform.localScale = new Vector3(30f, 30f, 30f);
				}
				else if (impact.gameObject.GetActive())
				{
					impact.gameObject.SetActive(value: false);
				}
				ren.startWidth = 1.5E-05f * World.transform.localScale.x * linerenmodifier;
				ren.endWidth = 1.5E-05f * World.transform.localScale.x * linerenmodifier;
			}
			if (islazer && (timepassed > 3f || start.gameObject == null || start == null || end == null))
			{
				F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
			}
		}
		if (timepassed > 15f)
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
		}
	}

	private void Destroythisgam()
	{
		if (islazer)
		{
			mispos = new Vector3(0f, 0f, 0f);
		}
		if ((bool)Explosion && (bool)Explosion.transform)
		{
			Transform transform = F3DPoolManager.Pools["GeneratedPool"].Spawn(Explosion.transform);
			if ((bool)transform)
			{
				GameObject gameObject = transform.gameObject;
				gameObject.transform.parent = base.transform.parent;
				gameObject.transform.position = base.transform.position;
				gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			}
		}
		F3DPoolManager.Pools["GeneratedPool"].Despawn(base.gameObject.transform);
	}

	public void startlineren(GameObject startin, GameObject endin, Material lazermat, float linemod, _Weapon.guntype inputgun, GameObject Sourcegunin)
	{
		islazer = true;
		start = startin;
		end = endin;
		ren = gameObject.GetComponent<LineRenderer>();
		ren.enabled = true;
		if ((bool)lazermat)
		{
			ren.material = lazermat;
		}
		linerenmodifier = linemod;
		guntype = inputgun;
		SourceGun = Sourcegunin;
	}

	public void isMissile()
	{
		ismissile = true;
	}
}
