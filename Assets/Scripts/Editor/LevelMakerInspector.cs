using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelMaker))]
public class LevelMakerInspector : Editor {

    private LevelMaker levelMaker;
    public override void OnInspectorGUI() {
        levelMaker = target as LevelMaker;

        DrawDefaultInspector();

        if(GUILayout.Button("Create Level Asset"))
        {
			levelMaker.SaveLevel();
        }
        if(GUILayout.Button("Display Level Asset"))
        {
			levelMaker.ShowLevel();
        }
        if(GUILayout.Button("Clear Scene"))
        {
			levelMaker.ClearScene();
        }
    }
}