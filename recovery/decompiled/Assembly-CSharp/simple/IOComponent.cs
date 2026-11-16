using System;
using System.Collections;
using UnityEngine;

namespace simple;

public class IOComponent : MonoBehaviour
{
	private IO _io = new IO();

	private Hashtable _data = new Hashtable();

	private string _dataStr;

	[SerializeField]
	private string _fileName;

	[SerializeField]
	private bool _useCompress;

	[SerializeField]
	private bool _dontDestoryOnLoad = true;

	public Hashtable data => _data;

	public string dataStr
	{
		get
		{
			return _dataStr;
		}
		set
		{
			_dataStr = value;
		}
	}

	public string fileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
		}
	}

	public bool useCompress
	{
		get
		{
			return _useCompress;
		}
		set
		{
			_useCompress = value;
		}
	}

	public bool dontDestroyOnLoad
	{
		get
		{
			return _dontDestoryOnLoad;
		}
		set
		{
			_dontDestoryOnLoad = value;
		}
	}

	private void Awake()
	{
		if (!_dontDestoryOnLoad)
		{
		}
	}

	public bool write()
	{
		if (!updateIO())
		{
			return false;
		}
		if (!_io.write(_data))
		{
			Debug.LogWarning("Write fail! : " + _io.debugString());
			return false;
		}
		return true;
	}

	public bool writeString(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if (!updateIO())
		{
			return false;
		}
		if (!_io.updateWrite(str))
		{
			Debug.LogWarning("Write fail! : " + _io.debugString());
			return false;
		}
		return true;
	}

	public void read()
	{
		if (updateIO())
		{
			Hashtable hashtable = _io.read();
			if (hashtable == null)
			{
				Debug.LogWarning("Read fail! : " + _io.debugString());
			}
			else
			{
				_data = hashtable;
			}
		}
	}

	public string readString()
	{
		if (!updateIO())
		{
			return null;
		}
		string text = _io.updateRead();
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Read fail! : " + _io.debugString());
		}
		_dataStr = text;
		return text;
	}

	private bool updateIO()
	{
		GameObject gameObject = transform.parent.gameObject;
		if (gameObject == null)
		{
			Debug.LogWarning("You must have parent GameObject!");
			return false;
		}
		IORoot component = gameObject.GetComponent<IORoot>();
		if (component == null)
		{
			Debug.LogWarning("You must have IORoot component!");
			return false;
		}
		_io.cryptoKey = component.cryptoKey;
		if (string.IsNullOrEmpty(_fileName))
		{
			Debug.LogWarning("Insert your file name!");
			return false;
		}
		_io.fileName = _fileName;
		_io.compress = _useCompress;
		return true;
	}

	public bool isEqualFile(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return false;
		}
		return fileName.Equals(_fileName);
	}

	public T get<T>(string key)
	{
		T result = default;
		if (key == null || !_data.ContainsKey(key))
		{
			return result;
		}
		switch (Convert.GetTypeCode(_data[key]))
		{
		case TypeCode.Object:
			result = (T)_data[key];
			break;
		default:
			result = (T)Convert.ChangeType(_data[key], Type.GetTypeCode(typeof(T)));
			break;
		case TypeCode.Empty:
		case TypeCode.DBNull:
			break;
		}
		return result;
	}

	public void add(object key, object value)
	{
		if (key != null && value != null)
		{
			if (_data.ContainsKey(key))
			{
				_data[key] = value;
			}
			else
			{
				_data.Add(key, value);
			}
		}
	}

	public void set(object key, object value)
	{
		if (key != null && value != null)
		{
			if (!_data.ContainsKey(key))
			{
				_data.Add(key, value);
			}
			else
			{
				_data[key] = value;
			}
		}
	}

	public void printData(string message)
	{
		string text = string.Empty;
		if (!string.IsNullOrEmpty(message))
		{
			text += $"[{message}]";
		}
		foreach (DictionaryEntry datum in _data)
		{
			text += $"{datum.Key}={datum.Value},";
		}
		Debug.Log(text);
	}

	public string debugInfo()
	{
		return _io.debugString();
	}
}
