using UnityEngine;

namespace HWRWeaponSystem;

[RequireComponent(typeof(AudioSource))]
public class WeaponLauncher : WeaponBase
{
	public bool OnActive;

	public bool OnScreenAiming;

	public Transform[] MissileOuter;

	public GameObject Missile;

	public float FireRate = 0.1f;

	private AILook al;

	public float Spread = 1f;

	public float ForceShoot = 8000f;

	public int NumBullet = 1;

	public int Ammo = 10;

	public int AmmoMax = 10;

	public bool InfinityAmmo;

	public float ReloadTime = 1f;

	public bool ShowHUD = true;

	public int MaxAimRange = 10000;

	public bool ShowCrosshair;

	public Texture2D CrosshairTexture;

	public Texture2D TargetLockOnTexture;

	public Texture2D TargetLockedTexture;

	public float DistanceLock = 200f;

	public float TimeToLock = 2f;

	public float AimDirection = 0.8f;

	public bool Seeker;

	public GameObject Shell;

	public float ShellLifeTime = 4f;

	public Transform[] ShellOuter;

	public int ShellOutForce = 300;

	public GameObject Muzzle;

	public float MuzzleLifeTime = 2f;

	public AudioClip[] SoundGun;

	public AudioClip SoundReloading;

	public AudioClip SoundReloaded;

	private float timetolockcount;

	private float nextFireTime;

	private GameObject target;

	private Vector3 torqueTemp;

	private float reloadTimeTemp;

	private AudioSource audioSource;

	[HideInInspector]
	public bool Reloading;

	[HideInInspector]
	public float ReloadingProcess;

	public GameObject CrosshairObject;

	private GameObject crosshair;

	public GameObject target1;

	[HideInInspector]
	public Vector3 AimPoint;

	[HideInInspector]
	public GameObject AimObject;

	public Camera CurrentCamera;

	private Vector3 crosshairPos;

	private int currentOuter;

	private void Start()
	{
		al = GetComponentInParent<AILook>();
		if (!Owner)
		{
			Owner = transform.root.gameObject;
		}
		if (al == null)
		{
			al = transform.root.GetComponent<AILook>();
		}
		if (!audioSource)
		{
			audioSource = GetComponent<AudioSource>();
			if (!audioSource)
			{
				gameObject.AddComponent<AudioSource>();
			}
		}
		if ((bool)CrosshairObject)
		{
			crosshair = Object.Instantiate(CrosshairObject.gameObject, transform.position, CrosshairObject.transform.rotation);
		}
	}

	private void rayAiming()
	{
		RaycastHit hitInfo;
		if (OnScreenAiming)
		{
			if (!CurrentCamera)
			{
				return;
			}
			Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo, MaxAimRange))
			{
				Debug.DrawLine(transform.position, hitInfo.point);
				if (Missile != null && hitInfo.collider.tag != Missile.tag)
				{
					AimPoint = hitInfo.point;
					AimObject = hitInfo.collider.gameObject;
				}
			}
			else
			{
				AimPoint = ray.origin + ray.direction * MaxAimRange;
				AimObject = null;
			}
		}
		else if (Physics.Raycast(transform.position, transform.forward, out hitInfo, MaxAimRange))
		{
			if (Missile != null && hitInfo.collider.tag != Missile.tag)
			{
				AimPoint = hitInfo.point;
				AimObject = hitInfo.collider.gameObject;
			}
		}
		else
		{
			AimPoint = transform.position + transform.forward * MaxAimRange;
			AimObject = null;
		}
	}

	private void FixedUpdate()
	{
		if (OnActive)
		{
			rayAiming();
		}
	}

	private void Update()
	{
		if (CurrentCamera == null)
		{
			CurrentCamera = Camera.main;
			if (CurrentCamera == null)
			{
				CurrentCamera = Camera.current;
			}
		}
		if (!OnActive)
		{
			return;
		}
		if ((bool)TorqueObject)
		{
			TorqueObject.transform.Rotate(torqueTemp * Time.deltaTime);
			torqueTemp = Vector3.Lerp(torqueTemp, Vector3.zero, Time.deltaTime);
		}
		if (Seeker)
		{
			for (int i = 0; i < TargetTag.Length; i++)
			{
				if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length <= 0)
				{
					continue;
				}
				GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
				float num = 2.1474836E+09f;
				if (AimObject != null && AimObject.tag == TargetTag[i])
				{
					float num2 = Vector3.Distance(AimObject.transform.position, transform.position);
					if (DistanceLock > num2 && num > num2 && timetolockcount + TimeToLock < Time.time)
					{
						num = num2;
						target = AimObject;
					}
					continue;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if ((bool)array[j])
					{
						Vector3 normalized = (array[j].transform.position - transform.position).normalized;
						float num3 = Vector3.Dot(normalized, transform.forward);
						float num4 = Vector3.Distance(array[j].transform.position, transform.position);
						if (num3 >= AimDirection && DistanceLock > num4 && num > num4 && timetolockcount + TimeToLock < Time.time)
						{
							num = num4;
							target = array[j];
						}
					}
				}
			}
		}
		if ((bool)target)
		{
			float num5 = Vector3.Distance(transform.position, target.transform.position);
			Vector3 normalized2 = (target.transform.position - transform.position).normalized;
			float num6 = Vector3.Dot(normalized2, transform.forward);
			if (num5 > DistanceLock || num6 <= AimDirection)
			{
				Unlock();
			}
		}
		if (Reloading)
		{
			ReloadingProcess = 1f / ReloadTime * (reloadTimeTemp + ReloadTime - Time.time);
			if (Time.time >= reloadTimeTemp + ReloadTime)
			{
				Reloading = false;
				if ((bool)SoundReloaded && (bool)audioSource)
				{
					audioSource.PlayOneShot(SoundReloaded);
				}
				Ammo = AmmoMax;
			}
		}
		else if (Ammo <= 0)
		{
			Unlock();
			Reloading = true;
			reloadTimeTemp = Time.time;
			if ((bool)SoundReloading && (bool)audioSource)
			{
				audioSource.PlayOneShot(SoundReloading);
			}
		}
	}

	private void DrawTargetLockon(Transform aimtarget, bool locked)
	{
		if (!ShowHUD || !CurrentCamera)
		{
			return;
		}
		if ((bool)crosshair)
		{
			crosshair.transform.position = AimPoint;
			crosshair.transform.forward = transform.forward;
		}
		Vector3 normalized = (aimtarget.position - CurrentCamera.transform.position).normalized;
		float num = Vector3.Dot(normalized, CurrentCamera.transform.forward);
		if (!(num > 0.5f))
		{
			return;
		}
		Vector3 vector = CurrentCamera.WorldToScreenPoint(aimtarget.transform.position);
		float f = Vector3.Distance(transform.position, aimtarget.transform.position);
		if (locked)
		{
			if ((bool)TargetLockedTexture)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)(TargetLockedTexture.width / 2), (float)Screen.height - vector.y - (float)(TargetLockedTexture.height / 2), TargetLockedTexture.width, TargetLockedTexture.height), TargetLockedTexture);
			}
			GUI.Label(new Rect(vector.x + 40f, (float)Screen.height - vector.y, 200f, 30f), aimtarget.name + " " + Mathf.Floor(f) + "m.");
		}
		else if ((bool)TargetLockOnTexture)
		{
			GUI.DrawTexture(new Rect(vector.x - (float)(TargetLockOnTexture.width / 2), (float)Screen.height - vector.y - (float)(TargetLockOnTexture.height / 2), TargetLockOnTexture.width, TargetLockOnTexture.height), TargetLockOnTexture);
		}
	}

	private void DrawCrosshair()
	{
		if (ShowCrosshair && (bool)CurrentCamera)
		{
			Vector3 vector = CurrentCamera.WorldToScreenPoint(AimPoint);
			crosshairPos += (vector - crosshairPos) / 5f;
			if ((bool)CrosshairTexture)
			{
				GUI.DrawTexture(new Rect(crosshairPos.x - (float)(CrosshairTexture.width / 2), (float)Screen.height - crosshairPos.y - (float)(CrosshairTexture.height / 2), CrosshairTexture.width, CrosshairTexture.height), CrosshairTexture);
			}
		}
	}

	private void OnGUI()
	{
		if (!OnActive)
		{
			return;
		}
		if (Seeker)
		{
			if ((bool)target)
			{
				DrawTargetLockon(target.transform, locked: true);
			}
			for (int i = 0; i < TargetTag.Length; i++)
			{
				if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length <= 0)
				{
					continue;
				}
				GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j])
					{
						continue;
					}
					Vector3 normalized = (array[j].transform.position - transform.position).normalized;
					float num = Vector3.Dot(normalized, transform.forward);
					if (num >= AimDirection)
					{
						float num2 = Vector3.Distance(array[j].transform.position, transform.position);
						if (DistanceLock > num2)
						{
							DrawTargetLockon(array[j].transform, locked: false);
						}
					}
				}
			}
		}
		DrawCrosshair();
	}

	private void Unlock()
	{
		timetolockcount = Time.time;
		target = null;
	}

	public void Shoot()
	{
		if (InfinityAmmo)
		{
			Ammo = 1;
		}
		if (Ammo <= 0 || !(Time.time > nextFireTime + FireRate))
		{
			return;
		}
		nextFireTime = Time.time;
		torqueTemp = TorqueSpeedAxis;
		Ammo--;
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		if (MissileOuter.Length > 0)
		{
			rotation = MissileOuter[currentOuter].rotation;
			position = MissileOuter[currentOuter].position;
		}
		if (MissileOuter.Length > 0)
		{
			currentOuter++;
			if (currentOuter >= MissileOuter.Length)
			{
				currentOuter = 0;
			}
		}
		if ((bool)Muzzle)
		{
			GameObject gameObject;
			if (WeaponSystem.Pool != null)
			{
				gameObject = WeaponSystem.Pool.Instantiate(Muzzle, position, rotation, MuzzleLifeTime);
			}
			else
			{
				gameObject = Object.Instantiate(Muzzle, position, rotation);
				Object.Destroy(gameObject, MuzzleLifeTime);
			}
			gameObject.transform.parent = base.transform;
			if (MissileOuter.Length > 0)
			{
				gameObject.transform.parent = MissileOuter[currentOuter].transform;
			}
		}
		for (int i = 0; i < NumBullet; i++)
		{
			if (!Missile)
			{
				continue;
			}
			Vector3 vector = new Vector3(Random.Range(0f - Spread, Spread), Random.Range(0f - Spread, Spread), Random.Range(0f - Spread, Spread)) / 100f;
			Vector3 vector2 = base.transform.forward + vector;
			rotation = Quaternion.LookRotation(vector2);
			GameObject gameObject2 = Object.Instantiate(Missile, position, rotation);
			MoverMissile component = gameObject2.GetComponent<MoverMissile>();
			if (component != null && (bool)al && (bool)al.target)
			{
				component.target1 = al.target;
			}
			if (!gameObject2)
			{
				continue;
			}
			DamageBase component2 = gameObject2.GetComponent<DamageBase>();
			if ((bool)component2)
			{
				component2.Owner = Owner;
				component2.TargetTag = TargetTag;
				component2.IgnoreTag = IgnoreTag;
				component2.IgnoreSelf(base.gameObject);
			}
			WeaponBase component3 = gameObject2.GetComponent<WeaponBase>();
			if ((bool)component3)
			{
				component3.Owner = Owner;
				component3.Target = target;
				component3.TargetTag = TargetTag;
				component3.IgnoreTag = IgnoreTag;
			}
			if (RigidbodyProjectile && (bool)gameObject2.GetComponent<Rigidbody>())
			{
				if (Owner != null && (bool)Owner.GetComponent<Rigidbody>())
				{
					gameObject2.GetComponent<Rigidbody>().velocity = Owner.GetComponent<Rigidbody>().velocity;
				}
				gameObject2.GetComponent<Rigidbody>().AddForce(vector2 * ForceShoot);
			}
			MoverMissile component4 = gameObject2.GetComponent<MoverMissile>();
			Damage component5 = gameObject2.GetComponent<Damage>();
			if ((bool)component5)
			{
				component5.selftag = base.transform.root.tag;
			}
			if (gameObject2.GetComponent("MoverMissile") as MoverMissile != null && target1 != null)
			{
				component4.target1 = target1;
			}
		}
		if ((bool)Shell)
		{
			Transform transform = base.transform;
			if (ShellOuter.Length > 0)
			{
				transform = ShellOuter[currentOuter];
			}
			GameObject gameObject3;
			if (WeaponSystem.Pool != null)
			{
				gameObject3 = WeaponSystem.Pool.Instantiate(Shell, transform.position, Random.rotation, ShellLifeTime);
			}
			else
			{
				gameObject3 = Object.Instantiate(Shell, transform.position, Random.rotation);
				Object.Destroy(gameObject3.gameObject, ShellLifeTime);
			}
			if ((bool)gameObject3.GetComponent<Rigidbody>())
			{
				gameObject3.GetComponent<Rigidbody>().AddForce(transform.forward * ShellOutForce);
			}
		}
		if (SoundGun.Length > 0 && (bool)audioSource)
		{
			audioSource.PlayOneShot(SoundGun[Random.Range(0, SoundGun.Length)]);
		}
		nextFireTime += FireRate;
	}
}
