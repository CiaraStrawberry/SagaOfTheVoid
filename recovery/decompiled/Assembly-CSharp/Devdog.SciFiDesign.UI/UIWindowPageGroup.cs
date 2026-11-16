using Devdog.General.UI;
using UnityEngine;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(UIWindow))]
public class UIWindowPageGroup : MonoBehaviour
{
	public UIWindowPage[] pages = new UIWindowPage[0];

	public bool loop;

	public KeyCode[] nextKeyCodes = new KeyCode[0];

	public KeyCode[] prevKeyCodes = new KeyCode[0];

	private int _index;

	private UIWindow _window;

	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = GetIndex(value);
		}
	}

	private int GetIndex(int index)
	{
		if (index < 0)
		{
			return pages.Length + index;
		}
		return index % pages.Length;
	}

	protected void Awake()
	{
		_window = GetComponent<UIWindow>();
	}

	protected void Start()
	{
		for (int i = 0; i < pages.Length; i++)
		{
			PrevWindowUI[] componentsInChildren = pages[i].gameObject.GetComponentsInChildren<PrevWindowUI>(includeInactive: true);
			if (componentsInChildren.Length > 0 && componentsInChildren[0].text != null)
			{
				if (i == 0 && !loop)
				{
					componentsInChildren[0].gameObject.SetActive(value: false);
				}
				else
				{
					componentsInChildren[0].text.text = pages[GetIndex(i - 1)].windowName;
				}
			}
			NextWindowUI[] componentsInChildren2 = pages[i].gameObject.GetComponentsInChildren<NextWindowUI>(includeInactive: true);
			if (componentsInChildren2.Length > 0 && componentsInChildren2[0].text != null)
			{
				if (i == pages.Length - 1 && !loop)
				{
					componentsInChildren2[0].gameObject.SetActive(value: false);
				}
				else
				{
					componentsInChildren2[0].text.text = pages[GetIndex(i + 1)].windowName;
				}
			}
			if (pages[i].isDefaultPage)
			{
				index = i;
			}
		}
	}

	protected void Update()
	{
		if (!_window.isVisible)
		{
			return;
		}
		KeyCode[] array = nextKeyCodes;
		foreach (KeyCode key in array)
		{
			if (Input.GetKeyDown(key))
			{
				NextPage();
			}
		}
		KeyCode[] array2 = prevKeyCodes;
		foreach (KeyCode key2 in array2)
		{
			if (Input.GetKeyDown(key2))
			{
				PrevPage();
			}
		}
	}

	public void NextPage()
	{
		if (index + 1 < pages.Length || loop)
		{
			index++;
			pages[index].Show();
		}
	}

	public void PrevPage()
	{
		if (index - 1 >= 0 || loop)
		{
			index--;
			pages[index].Show();
		}
	}
}
