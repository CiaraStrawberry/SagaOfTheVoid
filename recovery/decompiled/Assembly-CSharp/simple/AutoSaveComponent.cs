using System.Collections;
using UnityEngine;

namespace simple;

[RequireComponent(typeof(IOComponent))]
public class AutoSaveComponent : MonoBehaviour
{
	public enum eIOType
	{
		HashtableType,
		StringType
	}

	public delegate void onAutoSave();

	[SerializeField]
	public float _autoSaveTime = 30f;

	[SerializeField]
	public eIOType _ioType;

	public event onAutoSave eAutoSave;

	private void Start()
	{
		if (_autoSaveTime > 0f)
		{
			StartCoroutine("runAutoSave");
		}
	}

	private IEnumerator runAutoSave()
	{
		do
		{
			yield return new WaitForSeconds(_autoSaveTime);
			bool autoSaved = false;
			IOComponent io = gameObject.GetComponent<IOComponent>();
			if (io != null)
			{
				switch (_ioType)
				{
				case eIOType.HashtableType:
					autoSaved = io.write();
					break;
				case eIOType.StringType:
					autoSaved = io.writeString(io.dataStr);
					break;
				}
			}
			if (eAutoSave != null)
			{
				eAutoSave();
			}
			if (autoSaved)
			{
				Debug.Log("Auto saved : " + io.debugInfo());
			}
		}
		while (!(_autoSaveTime < 0f));
	}
}
