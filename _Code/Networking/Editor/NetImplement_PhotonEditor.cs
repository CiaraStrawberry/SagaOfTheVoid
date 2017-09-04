using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;

[Groups("System Settings", "Server Settings", "Packet Types")]
[CustomEditor(typeof(NetImplement_Photon))]
public class NetImplement_PhotonEditor : EasyEditorBase
{

    [Inspector(foldable = true, group = "Packet Types", rendererType = "CustomRenderer", groupDescription = "Packet handlers are used to serialize / deserialize packets intended for all clients")]
    private void RenderPackets() {

        DFNetwork.Networking.aNetworkLayer h = (DFNetwork.Networking.aNetworkLayer)target;

        EditorGUILayout.BeginVertical(DFNetwork.Helpers.EditorStyling.GetDefaultStyle);
        {

            #region User Packet Types

            // Get the available modules
            #region Show Packets

            // Update cache if nothing exists
            if (h.PacketRegistry.GetCommandNameList.Count == 0) {
                h.PacketRegistry.RefreshCache();
            }

            // Draw the GUI
            foreach (string nme in h.PacketRegistry.GetCommandNameList) {

                EditorGUILayout.LabelField(nme);

            }

            if (GUILayout.Button("Refresh Packet List")) {

                // Get the packet handler instance
                h.PacketRegistry.RefreshCache();

            }

            #endregion

            #endregion

        }
        EditorGUILayout.EndVertical();

    }

}