using System.Collections;
using UnityEngine;
using simple;

public class GameData : SerializedMonoBehaviour
{
	[LocalSerialized]
	private int _level = 1;

	[LocalSerialized]
	private int _score = 100;

	[LocalSerialized]
	private Hashtable _data = new Hashtable();

	[LocalSerialized]
	private ArrayList _list = new ArrayList();

	private int _tempLevel = 1;

	private void Start()
	{
		Debug.Log("GameData Level : " + _level);
		Debug.Log("GameData Temp Level : " + _tempLevel);
		Debug.Log("GameData Score : " + _score);
		StartCoroutine("runTestData");
	}

	private void Update()
	{
	}

	private IEnumerator runTestData()
	{
		int count = 0;
		while (true)
		{
			yield return new WaitForSeconds(5f);
			_tempLevel++;
			_level++;
			_score += 10;
			if (!_data.ContainsKey("desc"))
			{
				_data.Add("desc", "oh my god!");
			}
			ArrayList list = _list;
			int num;
			count = (num = count + 1);
			list.Add(num);
			IORoot.saveComponent(this);
		}
	}
}
