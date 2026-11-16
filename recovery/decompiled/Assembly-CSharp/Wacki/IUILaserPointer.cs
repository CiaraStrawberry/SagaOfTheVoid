using UnityEngine;

namespace Wacki;

public abstract class IUILaserPointer : MonoBehaviour
{
	public float laserThickness = 0.002f;

	public float laserHitScale = 0.02f;

	public bool laserAlwaysOn;

	public Color color;

	private GameObject hitPoint;

	private GameObject pointer;

	public LayerMask LayerFilter;

	private float _distanceLimit;

	private void Start()
	{
		LayerFilter = LayerMask.NameToLayer("UI");
		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pointer.transform.SetParent(transform, worldPositionStays: false);
		pointer.transform.localScale = new Vector3(laserThickness, laserThickness, 100f);
		pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hitPoint.transform.SetParent(transform, worldPositionStays: false);
		hitPoint.transform.localScale = new Vector3(laserHitScale, laserHitScale, laserHitScale);
		hitPoint.transform.localPosition = new Vector3(0f, 0f, 100f);
		hitPoint.SetActive(value: false);
		Object.DestroyImmediate(hitPoint.GetComponent<SphereCollider>());
		Object.DestroyImmediate(pointer.GetComponent<BoxCollider>());
		Material material = new Material(Shader.Find("Wacki/LaserPointer"));
		material.SetColor("_Color", color);
		pointer.GetComponent<MeshRenderer>().material = material;
		hitPoint.GetComponent<MeshRenderer>().material = material;
		Initialize();
		if (LaserPointerInputModule.instance == null)
		{
			new GameObject().AddComponent<LaserPointerInputModule>();
		}
		LaserPointerInputModule.instance.AddController(this);
	}

	private void OnDestroy()
	{
		if (LaserPointerInputModule.instance != null)
		{
			LaserPointerInputModule.instance.RemoveController(this);
		}
	}

	protected virtual void Initialize()
	{
	}

	public virtual void OnEnterControl(GameObject control)
	{
	}

	public virtual void OnExitControl(GameObject control)
	{
	}

	public abstract bool ButtonDown();

	public abstract bool ButtonUp();

	protected virtual void Update()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		bool flag = Physics.Raycast(ray, out var hitInfo, 1000f, LayerFilter);
		float num = 100f;
		if (flag)
		{
			num = hitInfo.distance;
		}
		if (_distanceLimit > 0f)
		{
			num = Mathf.Min(num, _distanceLimit);
			flag = true;
		}
		pointer.transform.localScale = new Vector3(laserThickness, laserThickness, num);
		pointer.transform.localPosition = new Vector3(0f, 0f, num * 0.5f);
		if (flag)
		{
			hitPoint.SetActive(value: true);
			hitPoint.transform.localPosition = new Vector3(0f, 0f, num);
		}
		else
		{
			hitPoint.SetActive(value: false);
		}
		_distanceLimit = -1f;
	}

	public virtual void LimitLaserDistance(float distance)
	{
		if (!(distance < 0f))
		{
			if (_distanceLimit < 0f)
			{
				_distanceLimit = distance;
			}
			else
			{
				_distanceLimit = Mathf.Min(_distanceLimit, distance);
			}
		}
	}
}
