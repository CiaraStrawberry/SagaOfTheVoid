using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
[CanEditMultipleObjects]
public class GameControllerEditor : UnityEditor.Editor {

    void OnSceneGUIdelay() {

        GameController gc = (GameController)target;
        Vector3 relPos = Vector3.zero;

        /* Spawn 1 */
        relPos = Vector3.zero - gc.SpawnPoint1;
        Handles.color = Color.blue;
        Handles.CubeCap(1, gc.SpawnPoint1, Quaternion.LookRotation(relPos), 1f);

        // Draw arrow
        Handles.ArrowCap(1, gc.SpawnPoint1, Quaternion.LookRotation(relPos), 5f);

        // Label it
        Handles.Label(new Vector3(gc.SpawnPoint1.x, 1, gc.SpawnPoint1.z), "Spawn 1");

        /* Spawn 2 */
        relPos = Vector3.zero - gc.SpawnPoint2;
        Handles.color = Color.green;
        Handles.CubeCap(2, gc.SpawnPoint2, Quaternion.LookRotation(relPos), 1f);

        // Draw arrow
        Handles.ArrowCap(2, gc.SpawnPoint2, Quaternion.LookRotation(relPos), 5f);

        // Label it
        Handles.Label(new Vector3(gc.SpawnPoint2.x, 1, gc.SpawnPoint2.z), "Spawn 2");

    }

    public void OnInspectorGUIdelay() {
        GUI.skin = EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector);
        DrawDefaultInspector();
    }

}
