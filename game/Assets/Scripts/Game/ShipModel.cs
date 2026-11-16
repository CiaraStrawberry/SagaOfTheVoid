using System;
using UnityEngine;

namespace Saga.Game
{
    // Sits on each ship prefab (built by ShipModelBuilder). Holds the ship's materials in every team colour.
    public class ShipModel : MonoBehaviour
    {
        [Serializable]
        public class Paint
        {
            public string colour;
            public Material[] materials; // one per renderer, in renderer order (each renderer has one slot)
        }

        public MeshRenderer[] renderers;
        public Paint[] paints;
        [Tooltip("Longest side of the model on the play plane, in model units.")]
        public float length;

        public static readonly string[] Colours = { "Black", "Blue", "Green", "Grey", "Red", "White", "Yellow" };

        public void ApplyPaint(string colour)
        {
            foreach (var p in paints)
            {
                if (p.colour != colour) continue;
                for (int i = 0; i < renderers.Length; i++) renderers[i].sharedMaterial = p.materials[i];
                return;
            }
            Debug.LogWarning(name + " has no " + colour + " paint");
        }
    }
}
