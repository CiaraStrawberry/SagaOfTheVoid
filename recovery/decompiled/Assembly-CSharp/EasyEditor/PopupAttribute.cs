using UnityEngine;

namespace EasyEditor;

public class PopupAttribute : PropertyAttribute
{
	public string[] list;

	public object variableType;

	public PopupAttribute(params object[] list)
	{
		if (IsVariablesTypeConsistent(list) && AssignVariableType(list[0]))
		{
			this.list = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				this.list[i] = list[i].ToString();
			}
		}
	}

	private bool AssignVariableType(object variable)
	{
		if (variable.GetType() == typeof(int))
		{
			variableType = typeof(int[]);
			return true;
		}
		if (variable.GetType() == typeof(float))
		{
			variableType = typeof(float[]);
			return true;
		}
		if (variable.GetType() == typeof(double))
		{
			Debug.LogWarning("Popup Drawer doesn't properly support double type, for float variables please use 'f' at the end of each value.");
			variableType = typeof(float[]);
			return true;
		}
		if (variable.GetType() == typeof(string))
		{
			variableType = typeof(string[]);
			return true;
		}
		Debug.LogError(string.Concat("Popup Property Drawer doesn't support ", variable.GetType(), " this type of variable"));
		return false;
	}

	private bool IsVariablesTypeConsistent(object[] list)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (i == 0)
			{
				variableType = list[i].GetType();
			}
			else if (variableType != list[i].GetType())
			{
				Debug.LogError("Popup Property Drawer can only contain one type per variable");
				return false;
			}
		}
		return true;
	}
}
