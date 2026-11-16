using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class fleetFileSerializer
{
	[Serializable]
	public class Ship
	{
		public string Type = string.Empty;

		public Vector3 Position;

		public string Weapon1;

		public string Weapon2;

		public string Weapon3;

		public string Weapon4;

		public string Weapon5;

		public Vector3 WeaponPos1;

		public Vector3 WeaponPos2;

		public Vector3 WeaponPos3;

		public Vector3 WeaponPos4;

		public Vector3 WeaponPos5;

		public Vector3 WeaponScale1;

		public Vector3 WeaponScale2;

		public Vector3 WeaponScale3;

		public Vector3 WeaponScale4;

		public Vector3 WeaponScale5;
	}

	public string FleetName = string.Empty;

	public List<Ship> Ships;

	public fleetFileSerializer()
	{
		Ships = new List<Ship>();
	}

	public static fleetFileSerializer Load(string jsonFile)
	{
		string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\";
		string text2 = File.ReadAllText(text + jsonFile);
		Debug.Log(text2);
		return JsonUtility.FromJson<fleetFileSerializer>(text2);
	}

	public static void Save(fleetFileSerializer handler, string jsonFile)
	{
		string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\";
		string contents = JsonUtility.ToJson(handler);
		File.WriteAllText(text + jsonFile, contents);
	}
}
