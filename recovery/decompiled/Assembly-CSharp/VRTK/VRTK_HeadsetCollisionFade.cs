using UnityEngine;

namespace VRTK;

[RequireComponent(typeof(VRTK_HeadsetCollision))]
[RequireComponent(typeof(VRTK_HeadsetFade))]
[AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetCollisionFade")]
public class VRTK_HeadsetCollisionFade : MonoBehaviour
{
	[Header("Collision Fade Settings")]
	[Tooltip("The amount of time to wait until a fade occurs.")]
	public float timeTillFade;

	[Tooltip("The fade blink speed on collision.")]
	public float blinkTransitionSpeed = 0.1f;

	[Tooltip("The colour to fade the headset to on collision.")]
	public Color fadeColor = Color.black;

	[Header("Custom Settings")]
	[Tooltip("The VRTK Headset Collision script to use when determining headset collisions. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_HeadsetCollision headsetCollision;

	[Tooltip("The VRTK Headset Fade script to use when fading the headset. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_HeadsetFade headsetFade;

	protected virtual void OnEnable()
	{
		headsetFade = ((!(headsetFade != null)) ? GetComponentInChildren<VRTK_HeadsetFade>() : headsetFade);
		headsetCollision = ((!(headsetCollision != null)) ? GetComponentInChildren<VRTK_HeadsetCollision>() : headsetCollision);
		headsetCollision.HeadsetCollisionDetect += OnHeadsetCollisionDetect;
		headsetCollision.HeadsetCollisionEnded += OnHeadsetCollisionEnded;
	}

	protected virtual void OnDisable()
	{
		headsetCollision.HeadsetCollisionDetect -= OnHeadsetCollisionDetect;
		headsetCollision.HeadsetCollisionEnded -= OnHeadsetCollisionEnded;
	}

	protected virtual void OnHeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
	{
		Invoke("StartFade", timeTillFade);
	}

	protected virtual void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
	{
		CancelInvoke("StartFade");
		headsetFade.Unfade(blinkTransitionSpeed);
	}

	protected virtual void StartFade()
	{
		headsetFade.Fade(fadeColor, blinkTransitionSpeed);
	}
}
