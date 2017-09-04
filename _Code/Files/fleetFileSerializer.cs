using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[Serializable]
public class fleetFileSerializer {

    [Serializable]
    public class Ship {

        public string Type = "";
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

    public string FleetName = "";
    public List<Ship> Ships;

    public fleetFileSerializer() {
        Ships = new List<Ship>();
    }

    /// <summary>
    /// Loads a fleet from disk
    /// </summary>
    /// <param name="jsonFile"></param>
    /// <returns></returns>
    public static fleetFileSerializer Load(string jsonFile) {
        string appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\";
        string json = System.IO.File.ReadAllText(appDataFolder + jsonFile);
        Debug.Log(json);
        return JsonUtility.FromJson<fleetFileSerializer>(json);
    }

    /// <summary>
    /// Saves a fleet to disk
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="jsonFile"></param>
    public static void Save(fleetFileSerializer handler, string jsonFile) {
        string appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\";
        string s = JsonUtility.ToJson(handler);
        System.IO.File.WriteAllText(appDataFolder + jsonFile, s);
    }

}
