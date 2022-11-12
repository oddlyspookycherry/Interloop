using UnityEngine;
using System.Collections.Generic;

public class RayGraph : MonoBehaviour
{
    public static RayGraph Instance { get; private set; }

    private Node previousNode, prePreviousNode;
    private List<Line> segments = new List<Line>();
    public Color nodeColor, segmentColor;

    public float segmentWidth;

    private void Awake()
    {
        Instance = this;
    }

    public void Clear()
    {
        segments.Clear();
        previousNode = prePreviousNode = null;
    }

    public void AddToGraph(Node node)
    {
        if (node.isInGraph)
            throw new System.Exception("Node already in graph.");
        else if (node == null)
            return;

        node.isInGraph = true;
        node.SetColor(nodeColor);
        if (previousNode != null)
        {
            GenerationManager.Instance.GenerateSegment(node.transform.localPosition,
                previousNode.transform.localPosition, segmentColor, segmentWidth);
            segments.Add(new Line(node.transform.localPosition, previousNode.transform.localPosition));
            prePreviousNode = previousNode;
        }

        previousNode = node;
    }

    public bool CompleteCycle(Node completingNode)
    {
        if (completingNode == null ||
            completingNode == previousNode ||
            completingNode == prePreviousNode)
            return false;
        GenerationManager.Instance.GenerateSegment(completingNode.transform.localPosition,
            previousNode.transform.localPosition, segmentColor, segmentWidth);
        segments.Add(new Line(completingNode.transform.localPosition, previousNode.transform.localPosition));
        return true;
    }

    public List<int> CheckForOverlap(List<Vector2> points, bool Inside)
    {
        List<int> result = new List<int>();
        if (Inside)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (CustomMaths.IsInsideArbitraryShape(segments, points[i]))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (!CustomMaths.IsInsideArbitraryShape(segments, points[i]))
            {
                result.Add(i);
            }
        }
        return result;
    }
}