using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationManager))]
public class GenerationManagerInspector : Editor {

    private GenerationManager manager;
    private void OnSceneGUI() {
        manager = target as GenerationManager;

        float halfX = manager.dimensions.x / 2f;
        float halfY = manager.dimensions.y / 2f;
        Vector2 center = GameState.totalOffsetVector;

        Vector2 point0 = new Vector2(center.x - halfX, center.y - halfY) + GameState.totalOffsetVector;
        Vector2 point1 = new Vector2(center.x - halfX, center.y + halfY) + GameState.totalOffsetVector;
        Vector2 point2 = new Vector2(center.x + halfX, center.y + halfY) + GameState.totalOffsetVector;
        Vector2 point3 = new Vector2(center.x + halfX, center.y - halfY) + GameState.totalOffsetVector;

        Handles.color = Color.cyan;

        Handles.DrawLine(point0, point1);
        Handles.DrawLine(point1, point2);
        Handles.DrawLine(point2, point3);
        Handles.DrawLine(point3, point0);

        Handles.color = Color.red;
        for(int i = 0; i < manager.triangles.Length / 3; i++)
        {
            Handles.DrawLine(manager.nodes[manager.triangles[i, 0]] + GameState.totalOffsetVector, manager.nodes[manager.triangles[i, 1]] + GameState.totalOffsetVector);
            Handles.DrawLine(manager.nodes[manager.triangles[i, 1]] + GameState.totalOffsetVector, manager.nodes[manager.triangles[i, 2]] + GameState.totalOffsetVector);
            Handles.DrawLine(manager.nodes[manager.triangles[i, 0]] + GameState.totalOffsetVector, manager.nodes[manager.triangles[i, 2]] + GameState.totalOffsetVector);
        }
    }

    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        var generationManager = (GenerationManager)target;

        if(GUILayout.Button("Remove Missing Objects"))
        {
            Undo.RecordObject(generationManager, "Remove Missing Objects");
            generationManager.RemoveMissingObjects();
        }
    }
}