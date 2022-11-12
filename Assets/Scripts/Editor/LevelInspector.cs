using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelInspector : Editor {
    public override void OnInspectorGUI() {
        Level level = target as Level;

        DrawDefaultInspector();

        if(GUILayout.Button("Reset"))
        {
            Undo.RecordObject(level, "Reset");
            level.nodes = new Vector2[0];
            level.keyDots = new Vector2[0];
            level.falseDots = new Vector2[0];
            level.portal = Vector2.zero;
            level.minTime = level.maxTime = 0f;
            EditorUtility.SetDirty(level);
        }
    }
}