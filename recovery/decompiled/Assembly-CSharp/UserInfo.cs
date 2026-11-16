using UnityEngine;
using simple;

public class UserInfo : SerializedMonoBehaviour
{
	[LocalSerialized]
	private string _userName = "John";

	[LocalSerialized]
	private int _age = 18;

	[LocalSerialized]
	private int _gender = 1;

	private void Start()
	{
		Debug.Log("UserInfo UserName : " + _userName);
		Debug.Log("UserInfo Age : " + _age);
		Debug.Log("UserInfo Gender : " + _gender);
		_userName = "Steive";
		_age = 30;
		_gender = 0;
		IORoot.saveComponent(this);
	}

	private void Update()
	{
	}
}
