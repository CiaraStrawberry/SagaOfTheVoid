using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saga.Game
{
    // Everything the battle view needs at runtime, in one asset (Assets/Resources/ShipLibrary.asset).
    // Built by ShipModelBuilder; referencing assets here also makes sure they ship in a build.
    [CreateAssetMenu(menuName = "Saga/Ship Library")]
    public class ShipLibrary : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string id;
            public GameObject prefab;
            public Sprite icon;
        }

        public List<Entry> ships = new List<Entry>();
        public TextAsset shipsJson;
        public TextAsset weaponsJson;
        public Shader fxShader;
        public Material skybox;

        public GameObject Prefab(string id)
        {
            foreach (var e in ships) if (e.id == id) return e.prefab;
            return null;
        }

        public Sprite Icon(string id)
        {
            foreach (var e in ships) if (e.id == id) return e.icon;
            return null;
        }

        public static ShipLibrary Load() => Resources.Load<ShipLibrary>("ShipLibrary");

        public Saga.Sim.Rulebook Rules() => new Saga.Sim.Rulebook(
            JsonUtility.FromJson<Saga.Sim.ShipDefList>(shipsJson.text),
            JsonUtility.FromJson<Saga.Sim.WeaponDefList>(weaponsJson.text));
    }
}
