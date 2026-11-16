using System.Collections;
using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

[RequireComponent(typeof(AudioSource))]
public class UnitShooter : MonoBehaviour
{
	[Required]
	public GameObject laserPrefab;

	public float laserSpeed = 10f;

	public AudioClipInfo audioClip;

	[Required]
	public Transform leftGun;

	[Required]
	public Transform rightGun;

	private AudioSource _audioSource;

	private GameObjectPool _laserPool;

	private WaitForSeconds _waitForSeconds;

	protected void Start()
	{
		_audioSource = gameObject.GetOrAddComponent<AudioSource>();
		_waitForSeconds = new WaitForSeconds(5f);
		_laserPool = new GameObjectPool(laserPrefab, 16);
		StartCoroutine(RandomUpdate());
	}

	private IEnumerator RandomUpdate()
	{
		while (true)
		{
			Shoot();
			yield return new WaitForSeconds(Random.Range(1f, 4f));
		}
	}

	protected void Update()
	{
		foreach (GameObject activeObjects in _laserPool.activeObjectsList)
		{
			if (!(activeObjects == null))
			{
				activeObjects.transform.Translate(Vector3.forward * Time.deltaTime * laserSpeed);
			}
		}
	}

	private void Shoot()
	{
		GameObject gameObject = _laserPool.Get();
		GameObject gameObject2 = _laserPool.Get();
		gameObject.transform.position = leftGun.position;
		gameObject.transform.rotation = leftGun.rotation;
		gameObject2.transform.position = rightGun.position;
		gameObject2.transform.rotation = rightGun.rotation;
		if (Vector3.Distance(transform.position, Camera.main.transform.position) < _audioSource.maxDistance)
		{
			_audioSource.Play(audioClip);
		}
		StartCoroutine(DestroyAfter(gameObject));
		StartCoroutine(DestroyAfter(gameObject2));
	}

	private IEnumerator DestroyAfter(GameObject laser)
	{
		yield return _waitForSeconds;
		_laserPool.Destroy(laser);
	}
}
