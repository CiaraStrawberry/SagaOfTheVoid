using UnityEngine;

public class WeaponBaseClass : MonoBehaviour
{
	private GameObject weapon;

	public GameObject Base;

	private Vector3 startpos;

	private Quaternion startrot;

	private Transform startparent;

	private Vector3 startscale;

	public TextMesh childtext;

	private float size = 1f;

	private bool noparent;

	public int cost;

	private _Ship ship;

	public bool equipment;

	public bool special;

	private bool late;

	private void Awake()
	{
		startpos = transform.position;
		startrot = transform.rotation;
		startscale = transform.localScale;
		if (!late)
		{
			startparent = transform.parent;
		}
	}

	private void Start()
	{
		ship = GetComponent<_Ship>();
		if ((bool)ship)
		{
			cost = ship.PointsToDeploy;
		}
		if (transform.parent == null)
		{
			Object.Destroy(gameObject);
			return;
		}
		if (transform.parent.parent.name != "Weapons" && transform.parent.parent.name != "ShipListParent" && ship == null)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else if (transform.parent.parent.name == "Weapons")
		{
			transform.localScale = GameObject.Find("ShipParent").GetComponent<MainMenuShipController>().scaleout() * 100f;
		}
		if ((bool)transform.Find("Description"))
		{
			childtext = transform.Find("Description").gameObject.GetComponent<TextMesh>();
		}
		if (late)
		{
			disabletext();
		}
		else
		{
			enabletext();
		}
	}

	private void Update()
	{
		if (transform.parent.parent.name == "Weapons")
		{
			transform.localScale = GameObject.Find("ShipParent").GetComponent<MainMenuShipController>().scaleout() * 100f;
		}
	}

	public bool valid()
	{
		return true;
	}

	public void startdif(float sizein, GameObject parentin)
	{
		size = sizein;
		late = true;
		startparent = parentin.transform;
	}

	public void respawn()
	{
		if (Base.transform.parent.Find(name) == null)
		{
			GameObject gameObject = Object.Instantiate(base.gameObject, Base.transform.position, Base.transform.rotation);
			gameObject.transform.parent = Base.transform.parent;
			if ((bool)ship)
			{
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				gameObject.transform.localScale = startscale;
			}
			gameObject.GetComponent<WeaponBaseClass>().size = size;
			gameObject.name = name;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public GameObject copy()
	{
		if (Base.transform.childCount < 1)
		{
			GameObject gameObject = Object.Instantiate(base.gameObject, Base.transform.position, Base.transform.rotation);
			gameObject.transform.localScale = startscale;
			gameObject.GetComponent<WeaponBaseClass>().startdif(size, startparent.gameObject);
			gameObject.name = name;
			return gameObject;
		}
		return base.gameObject;
	}

	public void reset(float sizeinput)
	{
		if ((Base.transform.childCount < 1 || transform.parent == Base.transform) && Base.transform.parent.Find(name) == null)
		{
			transform.position = startpos;
			transform.rotation = startrot;
			transform.parent = Base.transform.parent;
			if ((double)sizeinput >= 0.01)
			{
				size = sizeinput;
			}
			transform.localScale = startscale;
			Debug.Log(startscale);
		}
		else
		{
			Object.Destroy(gameObject);
		}
		enabletext();
	}

	public void enabletext()
	{
		if ((bool)childtext)
		{
			childtext.gameObject.SetActive(value: true);
		}
	}

	public void disabletext()
	{
		if ((bool)childtext)
		{
			childtext.gameObject.SetActive(value: false);
		}
	}

	public void scaletosize()
	{
		transform.localScale = new Vector3(size, size, size);
	}

	public void disabletextlate()
	{
		late = true;
	}

	public void startget()
	{
		if ((bool)transform.Find("Description"))
		{
			childtext = transform.Find("Description").gameObject.GetComponent<TextMesh>();
			late = true;
		}
	}
}
