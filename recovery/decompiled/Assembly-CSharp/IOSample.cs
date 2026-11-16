using System.Collections;
using UnityEngine;
using simple;

public class IOSample : MonoBehaviour
{
	private float _waitTime = 5f;

	private void Start()
	{
		write();
		read();
	}

	private void Update()
	{
		_waitTime -= Time.deltaTime;
		if (_waitTime <= 0f)
		{
			updateLevel();
			_waitTime = 5f;
		}
	}

	protected void updateLevel()
	{
		IOComponent iOComponent = IORoot.findIO("file_01");
		if (!(iOComponent == null))
		{
			int num = iOComponent.get<int>("level");
			iOComponent.set("level", num + 1);
			iOComponent.write();
			string text = iOComponent.get<string>("name");
			Hashtable hashtable = iOComponent.get<Hashtable>("optional");
			int num2 = (int)hashtable["age"];
			Debug.Log(text + ", " + num + "," + num2);
			read();
		}
	}

	protected void write()
	{
		IOComponent iOComponent = IORoot.findIO("file_01");
		if (!(iOComponent == null))
		{
			iOComponent.add("name", "John");
			iOComponent.add("level", 4);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("age", 10);
			bool flag = false;
			hashtable.Add("gender", flag);
			iOComponent.add("optional", hashtable);
			ArrayList arrayList = new ArrayList();
			arrayList.Add(5);
			arrayList.Add(10);
			iOComponent.add("step", arrayList);
			iOComponent.write();
		}
	}

	protected void read()
	{
		IOComponent iOComponent = IORoot.findIO("file_01");
		if (!(iOComponent == null))
		{
			iOComponent.read();
			iOComponent.printData("Read Data");
		}
	}
}
