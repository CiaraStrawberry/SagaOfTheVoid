using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Excelsior/CSFHI/Interface Animation Manager")]
public class InterfaceAnimManager : MonoBehaviour
{
	public List<InterfaceAnmElement> elementsList = new List<InterfaceAnmElement>();

	public bool autoStart = true;

	public bool invertDelays = true;

	public bool cloneDelays;

	public float timer;

	public int timeBetweenLoops = 3;

	public bool testLoop;

	public bool autoLinearAppearDelay = true;

	public float appearDelay_SpeedMultiplier = 1f;

	public float disappearDelay_SpeedMultiplier = 1f;

	public CSFHIAnimableState currentState = CSFHIAnimableState.disappeared;

	public bool useDelays = true;

	public AudioSource audioSource;

	public AudioClip openSound;

	public AudioClip closeSound;

	public List<InterfaceAnimManager> nestedIAM = new List<InterfaceAnimManager>();

	public bool waitAppear_Nested;

	public bool waitDisappear_Nested;

	private InterfaceAnmElement waitElementFullAnim;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			UpdateAnimClips();
			startDisappear(direct: true);
			if (autoStart)
			{
				startAppear();
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		switch (currentState)
		{
		case CSFHIAnimableState.appearing:
			if (waitAppear_Nested && waitElementFullAnim != null)
			{
				if ((bool)waitElementFullAnim.gameObjectRef && waitElementFullAnim.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appeared)
				{
					waitElementFullAnim = null;
				}
				break;
			}
			foreach (InterfaceAnmElement elements in elementsList)
			{
				Animator component = elements.gameObjectRef.GetComponent<Animator>();
				if ((bool)component)
				{
					for (int i = 0; i < component.layerCount; i++)
					{
						component.SetLayerWeight(i, 1f);
					}
				}
				if (!useDelays || timer > elements.timeAppear * appearDelay_SpeedMultiplier)
				{
					if (elements.currentState != CSFHIAnimableState.disappeared)
					{
						continue;
					}
					elements.gameObjectRef.SetActive(value: true);
					if ((bool)elements.animator)
					{
						AnimationClip[] animClips = elements.animClips;
						int num = 0;
						if (num < animClips.Length)
						{
							AnimationClip animationClip = animClips[num];
							if (animationClip.name.ToLower().Contains("appear"))
							{
								elements.animator.Play(animationClip.name, -1, 0f);
							}
						}
					}
					if (elements.isNested_IAM)
					{
						if (waitAppear_Nested)
						{
							waitElementFullAnim = elements;
						}
						elements.gameObjectRef.GetComponent<InterfaceAnimManager>().startAppear();
					}
					elements.currentState = CSFHIAnimableState.appearing;
				}
				else if (!useDelays)
				{
				}
			}
			timer += Time.deltaTime;
			if ((testLoop && timer > appearDelay_SpeedMultiplier + (float)timeBetweenLoops) || (!testLoop && timer > appearDelay_SpeedMultiplier + 1f))
			{
				endAppear();
			}
			break;
		case CSFHIAnimableState.disappearing:
		{
			if (waitDisappear_Nested && waitElementFullAnim != null)
			{
				if (waitElementFullAnim.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.disappeared)
				{
					waitElementFullAnim = null;
				}
				break;
			}
			foreach (InterfaceAnmElement elements2 in elementsList)
			{
				if (useDelays && !(timer > elements2.timeDisappear * disappearDelay_SpeedMultiplier))
				{
					continue;
				}
				if (elements2.currentState != CSFHIAnimableState.disappearing && elements2.currentState != CSFHIAnimableState.disappeared)
				{
					if (elements2.isNested_IAM)
					{
						if (elements2.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appeared || elements2.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appearing)
						{
							elements2.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear();
							elements2.currentState = CSFHIAnimableState.disappearing;
							if (waitDisappear_Nested)
							{
								waitElementFullAnim = elements2;
							}
						}
					}
					else
					{
						bool flag = false;
						if ((bool)elements2.animator)
						{
							AnimationClip[] animClips2 = elements2.animClips;
							foreach (AnimationClip animationClip2 in animClips2)
							{
								if (!animationClip2.name.ToLower().Contains("disappear"))
								{
									continue;
								}
								if ((bool)elements2.animator)
								{
									for (int k = 0; k < elements2.animator.layerCount; k++)
									{
										elements2.animator.SetLayerWeight(k, 0f);
									}
									elements2.currentState = CSFHIAnimableState.disappearing;
									elements2.animator.Play(animationClip2.name);
									flag = true;
								}
								break;
							}
						}
						if (!flag)
						{
							elements2.gameObjectRef.SetActive(value: false);
							elements2.currentState = CSFHIAnimableState.disappeared;
						}
					}
				}
				if (elements2.isNested_IAM || elements2.currentState != CSFHIAnimableState.disappearing)
				{
					continue;
				}
				string empty = string.Empty;
				AnimationClip[] animClips3 = elements2.animClips;
				foreach (AnimationClip animationClip3 in animClips3)
				{
					if (animationClip3.name.ToLower().Contains("disappear"))
					{
						empty = animationClip3.name;
						break;
					}
				}
				if (elements2.animator.GetCurrentAnimatorStateInfo(0).IsName(empty) && elements2.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				{
					elements2.currentState = CSFHIAnimableState.disappeared;
					elements2.gameObjectRef.SetActive(value: false);
				}
			}
			bool flag2 = true;
			foreach (InterfaceAnmElement elements3 in elementsList)
			{
				if (elements3.currentState != CSFHIAnimableState.disappearing)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				StartCoroutine(Disable_OnDisappearEnd());
			}
			timer += Time.deltaTime;
			foreach (InterfaceAnmElement elements4 in elementsList)
			{
				if (elements4.isNested_IAM && elements4.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState != CSFHIAnimableState.disappeared)
				{
					return;
				}
			}
			if ((bool)audioSource && audioSource.enabled && audioSource.clip != closeSound)
			{
				audioSource.clip = closeSound;
				audioSource.Play();
			}
			if ((!testLoop || !(timer >= disappearDelay_SpeedMultiplier + (float)timeBetweenLoops)) && (testLoop || !(timer >= disappearDelay_SpeedMultiplier)))
			{
				break;
			}
			foreach (InterfaceAnmElement elements5 in elementsList)
			{
				if ((elements5.isNested_IAM && elements5.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState != CSFHIAnimableState.disappeared) || (!elements5.isNested_IAM && elements5.currentState != CSFHIAnimableState.disappeared))
				{
					return;
				}
			}
			endDisappear();
			break;
		}
		case CSFHIAnimableState.appeared:
		case CSFHIAnimableState.disappeared:
			if (testLoop)
			{
				if (currentState == CSFHIAnimableState.appeared)
				{
					startDisappear();
				}
				if (currentState == CSFHIAnimableState.disappeared)
				{
					startAppear();
				}
			}
			break;
		}
	}

	public IEnumerator Disable_OnDisappearEnd()
	{
		int longestWait = 1;
		foreach (InterfaceAnmElement elements in elementsList)
		{
			if (elements.gameObjectRef.activeSelf && elements.animator != null && longestWait < elements.animator.GetCurrentAnimatorClipInfo(0).Length)
			{
				longestWait = elements.animator.GetCurrentAnimatorClipInfo(0).Length;
			}
		}
		yield return new WaitForSeconds(longestWait);
		int index = 0;
		foreach (InterfaceAnmElement elements2 in elementsList)
		{
			index++;
			if (elements2.isNested_IAM)
			{
				elements2.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear();
			}
		}
		StopCoroutine(Disable_OnDisappearEnd());
		if (!isIAM_Root() && (bool)gameObject.transform.parent.GetComponent<InterfaceAnimManager>())
		{
			endDisappear();
		}
	}

	public virtual void startAppear(bool direct = false)
	{
		if (currentState == CSFHIAnimableState.appeared)
		{
			return;
		}
		if (currentState == CSFHIAnimableState.disappearing)
		{
			Debug.LogWarning("CSFHI : start appear not available if disappear in progress");
		}
		else
		{
			if (currentState == CSFHIAnimableState.appearing)
			{
				return;
			}
			foreach (InterfaceAnmElement elements in elementsList)
			{
				if ((bool)elements && elements.isNested_IAM)
				{
					elements.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear(direct: true);
					elements.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState = CSFHIAnimableState.disappeared;
				}
			}
			gameObject.SetActive(value: true);
			if (!direct && (bool)audioSource && audioSource.enabled && currentState != CSFHIAnimableState.appeared && currentState != CSFHIAnimableState.appearing)
			{
				audioSource.clip = openSound;
				audioSource.Play();
			}
			if (direct)
			{
				foreach (InterfaceAnmElement elements2 in elementsList)
				{
					if ((bool)elements2)
					{
						elements2.gameObjectRef.SetActive(value: true);
						if (elements2.isNested_IAM)
						{
							elements2.gameObjectRef.GetComponent<InterfaceAnimManager>().startAppear(direct: true);
						}
						elements2.currentState = CSFHIAnimableState.appearing;
					}
				}
				endAppear();
			}
			else
			{
				timer = 0f;
				currentState = CSFHIAnimableState.appearing;
			}
		}
	}

	private void endAppear()
	{
		currentState = CSFHIAnimableState.appeared;
		waitElementFullAnim = null;
	}

	public virtual void startDisappear(bool direct = false)
	{
		if (currentState == CSFHIAnimableState.appeared)
		{
			timer = 0f;
		}
		if (currentState == CSFHIAnimableState.appearing)
		{
			timer = disappearDelay_SpeedMultiplier - timer;
		}
		if (!direct && currentState == CSFHIAnimableState.disappearing)
		{
			return;
		}
		if (direct)
		{
			foreach (InterfaceAnmElement elements in elementsList)
			{
				if (elements.isNested_IAM)
				{
					elements.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear(direct: true);
				}
				elements.gameObjectRef.SetActive(value: false);
				elements.currentState = CSFHIAnimableState.disappearing;
			}
			endDisappear();
		}
		else
		{
			if (waitAppear_Nested)
			{
				waitElementFullAnim = null;
			}
			currentState = CSFHIAnimableState.disappearing;
		}
	}

	private void endDisappear()
	{
		foreach (InterfaceAnmElement elements in elementsList)
		{
			elements.currentState = CSFHIAnimableState.disappeared;
			if (elements.isNested_IAM)
			{
				elements.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState = CSFHIAnimableState.disappeared;
			}
			elements.gameObjectRef.SetActive(value: false);
		}
		waitElementFullAnim = null;
		StopCoroutine(Disable_OnDisappearEnd());
		currentState = CSFHIAnimableState.disappeared;
		if (isIAM_Root())
		{
			return;
		}
		gameObject.SetActive(value: false);
		InterfaceAnimManager component = transform.parent.GetComponent<InterfaceAnimManager>();
		if (component.currentState != CSFHIAnimableState.disappearing || ((bool)component.transform.parent && (bool)component.transform.parent.GetComponent<InterfaceAnimManager>()))
		{
			return;
		}
		int num = 0;
		foreach (InterfaceAnmElement elements2 in component.elementsList)
		{
			if (elements2.gameObjectRef == gameObject)
			{
				if (num == 0)
				{
					component.endDisappear();
				}
				break;
			}
			num++;
		}
	}

	public void UpdateAnimClips()
	{
		bool flag = true;
		int num = 0;
		foreach (Transform item2 in gameObject.transform)
		{
			if (!(item2.parent == gameObject.transform))
			{
				continue;
			}
			flag = true;
			for (int num2 = elementsList.Count - 1; num2 >= 0; num2--)
			{
				if (elementsList[num2] == null || elementsList[num2].gameObjectRef == null || elementsList[num2].gameObjectRef.transform.parent != gameObject.transform)
				{
					if (elementsList[num2] != null)
					{
						elementsList[num2].Delete();
					}
					elementsList.Remove(elementsList[num2]);
				}
				else if (elementsList[num2].gameObjectRef == item2.gameObject)
				{
					flag = false;
					elementsList[num2].sortID = num;
					break;
				}
			}
			if (flag)
			{
				InterfaceAnmElement item = ((!item2.gameObject.GetComponent<InterfaceAnimManager>()) ? InterfaceAnmElement.Create(item2.gameObject, num, _isNested_IAM: false) : InterfaceAnmElement.Create(item2.gameObject, num, _isNested_IAM: true));
				elementsList.Add(item);
			}
			num++;
		}
		elementsList = elementsList.OrderBy((InterfaceAnmElement x) => x.sortID).ToList();
		if (!isIAM_Root())
		{
			foreach (InterfaceAnimManager item3 in nestedIAM)
			{
				item3.autoStart = autoStart;
				item3.useDelays = useDelays;
				item3.timeBetweenLoops = timeBetweenLoops;
				item3.waitAppear_Nested = waitAppear_Nested;
				item3.waitDisappear_Nested = waitDisappear_Nested;
			}
		}
		else
		{
			nestedIAM = GetComponentsInChildren<InterfaceAnimManager>(includeInactive: true).ToList();
			nestedIAM.Remove(this);
			foreach (InterfaceAnimManager item4 in nestedIAM)
			{
				item4.autoStart = autoStart;
				item4.useDelays = useDelays;
				item4.timeBetweenLoops = timeBetweenLoops;
				item4.waitAppear_Nested = waitAppear_Nested;
				item4.waitDisappear_Nested = waitDisappear_Nested;
			}
		}
		foreach (InterfaceAnmElement elements in elementsList)
		{
			if ((bool)elements)
			{
				elements.UpdateProperties();
			}
		}
		if (!isIAM_Root())
		{
			return;
		}
		int num3 = 0;
		foreach (InterfaceAnimManager item5 in nestedIAM)
		{
			if ((bool)item5.gameObject.GetComponent<InterfaceAnimManager>())
			{
				item5.gameObject.name = "Nested_IAM[" + num3 + "]";
			}
			num3++;
		}
		foreach (InterfaceAnimManager item6 in nestedIAM)
		{
			item6.UpdateAnimClips();
		}
	}

	public bool isIAM_Root()
	{
		if ((bool)transform.parent && (bool)transform.parent.GetComponent<InterfaceAnimManager>())
		{
			return false;
		}
		return true;
	}
}
