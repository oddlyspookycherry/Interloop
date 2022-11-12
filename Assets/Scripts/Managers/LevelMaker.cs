#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
public class LevelMaker : MonoBehaviour
{
    public Level levelToFill;
    public GameObject nodePrefab, dotPrefab, falseDotPrefab, portalPrefab;

    public bool isEditing;
    public void SaveLevel()
    {
        if(levelToFill.nodeCount > 0 && !isEditing)
            throw new System.Exception("You are trying to override an existing level.");

        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        GameObject[] keyDots = GameObject.FindGameObjectsWithTag("KeyDot");
        GameObject[] falseDots = GameObject.FindGameObjectsWithTag("FalseDot");
        GameObject portal = GameObject.FindGameObjectWithTag("Portal");
        Vector2[] nodPoses = new Vector2[nodes.Length];
        Vector2[] keyDotPoses = new Vector2[keyDots.Length];
        Vector2[] falseDotPoses = new Vector2[falseDots.Length];

        if(nodes.Length == 0 || keyDots.Length == 0 || portal == null)
            throw new System.Exception("Invalid Level");
 
        for(int i = 0; i < nodPoses.Length; i++)
            nodPoses[i] = nodes[i].transform.position;
        for (int i = 0; i < keyDotPoses.Length; i++)
            keyDotPoses[i] = keyDots[i].transform.position;
        for(int i = 0; i < falseDotPoses.Length; i++)
            falseDotPoses[i] = falseDots[i].transform.position;
        

        levelToFill.nodes = nodPoses;
        levelToFill.keyDots = keyDotPoses;
        levelToFill.falseDots = falseDotPoses;
        levelToFill.portal = portal.transform.position;

        EditorUtility.SetDirty(levelToFill);

        Debug.LogFormat("Level saved in '{0}'.", levelToFill.name);
    }

    public void ShowLevel()
    {
        isEditing = true;
        for(int i = 0; i < levelToFill.nodeCount; i++)
            Instantiate(nodePrefab, levelToFill.nodes[i], Quaternion.identity);
        for(int i = 0; i < levelToFill.keyDotCount; i++)
            Instantiate(dotPrefab, levelToFill.keyDots[i], Quaternion.identity);
        for(int i = 0; i < levelToFill.falseDotCount; i++)
            Instantiate(falseDotPrefab, levelToFill.falseDots[i], Quaternion.identity);
        Instantiate(portalPrefab, levelToFill.portal, Quaternion.identity);
        Debug.LogFormat("Level {0} shown.", levelToFill.name);
    }

    public void ClearScene()
    {
        isEditing = false;
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        GameObject[] keyDots = GameObject.FindGameObjectsWithTag("KeyDot");
        GameObject[] falseDots = GameObject.FindGameObjectsWithTag("FalseDot");
        GameObject portal = GameObject.FindGameObjectWithTag("Portal");
        foreach(GameObject o in nodes)
            DestroyImmediate(o);
        foreach(GameObject o in keyDots)
            DestroyImmediate(o);
        foreach(GameObject o in falseDots)
            DestroyImmediate(o);
        DestroyImmediate(portal);
        Debug.Log("Scene Cleared");
    }
}
#endif