using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;

[Groups("Engine Settings", "Hull Settings", "Model Settings", "Basic Stats")]
[CustomEditor(typeof(Destroyer_Tank))]
public class Destroyer_TankEditor : EasyEditorBase
{

    [Inspector(group = "Basic Stats", rendererType = "CustomRenderer")]
    private void RenderProgressBar() {

        //Debug.Log("SADF");

        _Ship ship = (_Ship)target;

        // Get the weapons object
        if (ship.WeaponsObject != null) {

            // Find all weapon slot objects within
            WeaponSlot[] slots = ship.WeaponsObject.GetComponentsInChildren<WeaponSlot>(false);

            Rect r = EditorGUILayout.BeginVertical();

            foreach (WeaponSlot ws in slots) {

                EditorGUILayout.ObjectField(ws.WeaponToPlace, typeof(_Weapon), true);

            }

            //EditorGUI.ProgressBar(r, 0.8f, "Life");
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();

        }

    }

}