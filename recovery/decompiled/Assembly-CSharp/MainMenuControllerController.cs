using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class MainMenuControllerController : MonoBehaviour
{
	private VRTK_ControllerEvents eventscon;

	private LineRenderer lineren;

	public ButtonPressedMessageSender hitobj;

	public AudioClip audioahptic;

	public GameObject lastobj;

	private RightHand_triggerInstantOrder righthandinstant;

	private int framestiilnexthaptic;

	private int minhaplimit = 60;

	public GameObject ViveObject;

	private AudioSource audio;

	private CrossLevelVariableHolder crossvar;

	private void Start()
	{
		eventscon = GetComponent<VRTK_ControllerEvents>();
		eventscon.TriggerPressed += trigger;
		lineren = GetComponent<LineRenderer>();
		lineren.startWidth = 0.002f;
		lineren.endWidth = 0.002f;
		lineren.enabled = false;
		righthandinstant = GetComponent<RightHand_triggerInstantOrder>();
		audio = GetComponent<AudioSource>();
	}

	private void Update()
	{
		framestiilnexthaptic++;
		if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1000f))
		{
			ButtonPressedMessageSender component = hitInfo.transform.gameObject.GetComponent<ButtonPressedMessageSender>();
			if (component != null)
			{
				if (hitInfo.collider.gameObject != lastobj)
				{
					TriggerHapticPulse();
					lastobj = hitInfo.collider.gameObject;
				}
				lineren.enabled = true;
				lineren.SetPosition(0, transform.position);
				lineren.SetPosition(1, hitInfo.point);
				hitobj = component;
				ButtonHoverMessageSender component2 = hitInfo.transform.gameObject.GetComponent<ButtonHoverMessageSender>();
				if (component2 != null)
				{
					component2.SendMessage();
				}
			}
			else
			{
				disablelineren();
			}
		}
		else
		{
			hitobj = null;
			disablelineren();
		}
	}

	private void TriggerHapticPulse()
	{
		Debug.Log("hapticpulse");
		if (ViveObject == null || !ViveObject.GetActive())
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerIndex(gameObject)), audioahptic);
		}
	}

	private void disablelineren()
	{
		if (this != null && gameObject != null && (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6))
		{
			lineren.enabled = false;
		}
	}

	private void trigger(object sender, ControllerInteractionEventArgs e)
	{
		if ((bool)hitobj && hitobj.gameObject != null)
		{
			audio.Play();
			hitobj.sendmessage();
		}
	}
}
