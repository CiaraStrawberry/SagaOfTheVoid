using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EasyEditor;
using DFNetwork.Simulation;

[Groups("Simulation Settings", "Modules", "Resources")]
[CustomEditor(typeof(SimulationLayer))]
public class SimulationLayerEditor : EasyEditorBase {

    [Inspector(displayHeader = true, foldable = true, group = "Modules", rendererType = "CustomRenderer", groupDescription = "All modules that handle packets currently in the system")]
    private void RenderModules() {

        SimulationLayer sl = (SimulationLayer)target;

        EditorGUILayout.BeginVertical(DFNetwork.Helpers.EditorStyling.GetDefaultStyle);
        {

            #region User Packet Types

            // Get the available modules
            #region Show Packets

            // Update cache if nothing exists
            if (sl.ModuleRegistry.GetModuleNameList.Count == 0) {
                sl.ModuleRegistry.RefreshCache();
            }

            // Draw the GUI
            foreach (string nme in sl.ModuleRegistry.GetModuleNameList) {

                EditorGUILayout.LabelField(nme);

            }

            if (GUILayout.Button("Refresh module List")) {

                // Get the packet handler instance
                sl.ModuleRegistry.RefreshCache();

            }

            #endregion

            #endregion

        }
        EditorGUILayout.EndVertical();

    }

    [Inspector(displayHeader = true, foldable = true, group = "Resources", rendererType = "CustomRenderer", groupDescription = "Listing of all resources usable by the simulation engine.  If your object isn't in the list, it is because it is missing a subtype of _Moveable / _Ship / _Station / _Stationary / _Weapon")]
    private void RenderResources() {

        SimulationLayer sl = (SimulationLayer)target;

        EditorGUILayout.HelpBox("Please note that refreshing may change ID's, don't hard code id's.  If you don't see your prefab after refreshing, make sure it implements a class that inherits \"aObject\"", UnityEditor.MessageType.Info);
        if (GUILayout.Button("Refresh Resource List")) {

            // Get the latest resources
            sl.Resources = DFNetwork.Simulation.Resources.ResourceList.GetAllResources;

        }

        // Draw resources
        foreach (ResourceItem ri in sl.Resources) {
            EditorGUILayout.LabelField(ri.PrefabID.ToString("D4") + "\t " + ri.PrefabName);
        }

    }

}
