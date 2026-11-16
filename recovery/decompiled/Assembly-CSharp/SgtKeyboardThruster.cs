using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Keyboard Thruster")]
public class SgtKeyboardThruster : MonoBehaviour
{
	[Serializable]
	public class Group
	{
		[Tooltip("The control axis used for these thrusters")]
		public string Axis;

		public bool Inverse;

		public bool Bidirectional;

		public List<SgtThruster> Thrusters;
	}

	public List<Group> Groups = new List<Group>();

	protected virtual void Update()
	{
		if (Groups == null)
		{
			return;
		}
		for (int num = Groups.Count - 1; num >= 0; num--)
		{
			Group obj = Groups[num];
			if (obj != null)
			{
				float num2 = Input.GetAxisRaw(obj.Axis);
				if (obj.Inverse)
				{
					num2 = 0f - num2;
				}
				if (!obj.Bidirectional && num2 < 0f)
				{
					num2 = 0f;
				}
				for (int num3 = obj.Thrusters.Count - 1; num3 >= 0; num3--)
				{
					SgtThruster sgtThruster = obj.Thrusters[num3];
					if (sgtThruster != null)
					{
						sgtThruster.Throttle = num2;
					}
				}
			}
		}
	}
}
