using ParticlePlayground;
using UnityEngine;

public class PlaygroundEventListener : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	private PlaygroundEventC playgroundEvent;

	private Transform thisTransform;

	private Collider thisCollider;

	private Renderer thisRenderer;

	private float localAxisRotation;

	private float collisionAmplifier = 20f;

	private float rotationDamping = 1f;

	private bool isActive = true;

	private static Material activeMaterial;

	private static Material inactiveMaterial;

	private void Start()
	{
		playgroundEvent = PlaygroundC.GetEvent(0, particles);
		AddEventListener();
		thisTransform = transform;
		thisCollider = GetComponent<Collider>();
		thisRenderer = GetComponent<Renderer>();
		if (activeMaterial == null)
		{
			activeMaterial = new Material(Shader.Find("Diffuse"));
			activeMaterial.color = Color.white;
		}
		if (inactiveMaterial == null)
		{
			inactiveMaterial = new Material(Shader.Find("Diffuse"));
			inactiveMaterial.color = Color.black;
		}
		thisRenderer.sharedMaterial = activeMaterial;
	}

	private void ParticleEvent(PlaygroundEventParticle particle)
	{
		if (particle.collisionCollider == thisCollider)
		{
			localAxisRotation += particle.size * particle.velocity.magnitude * collisionAmplifier;
		}
	}

	private void AddEventListener()
	{
		playgroundEvent.particleEvent += ParticleEvent;
	}

	private void RemoveEventListener()
	{
		playgroundEvent.particleEvent -= ParticleEvent;
	}

	private void Update()
	{
		thisTransform.RotateAround(thisTransform.position, thisTransform.forward, (0f - localAxisRotation) * PlaygroundC.globalDeltaTime);
		localAxisRotation = Mathf.Lerp(localAxisRotation, 0f, rotationDamping * PlaygroundC.globalDeltaTime);
	}

	private void OnMouseUp()
	{
		isActive = !isActive;
		thisRenderer.sharedMaterial = ((!isActive) ? inactiveMaterial : activeMaterial);
		if (isActive)
		{
			AddEventListener();
		}
		else
		{
			RemoveEventListener();
		}
	}
}
