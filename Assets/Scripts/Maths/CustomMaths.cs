using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class CustomMaths
{
    private const float eps = 0.0001f;
    public static bool Compare(float a, float b)
    {
        return Mathf.Abs(a - b) < eps;
    }
    public static bool Compare(Vector2 a, Vector2 b)
    {
        return Compare(a.x, b.x) && Compare(a.y, b.y);
    }

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static bool IsOnLine(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return Compare(Cross(p - p1, p2 - p1), 0f) &&
            Dot(p - p1, p2 - p1) >= 0f &&
            Dot(p - p2, p1 - p2) >= 0f;
    }

    public static float AngleABC(Vector2 A, Vector2 B, Vector2 C)
    {
        A = A - B;
        C = C - B;
        return Mathf.Atan2(Cross(A, C), Dot(A, C));
    }

    public static float OrientatedAngle(Vector2 x, Vector2 y)
    {
        return Mathf.Atan2(Cross(x, y), Dot(x, y));
    }

    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle));
    }
    public static Vector2 LineIntersection(Line f, Line s)
    {
        if(Compare(Cross(f.normal, s.normal), 0f))
        {
            return new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        }

        float resx, resy;
        if (s.a == 0 || s.b == 0)
        {
            GeneralMethods.Swap<Line>(ref f, ref s);
        }
        if (f.a == 0)
        {
            resy = -(f.c / f.b);
            resx = -(s.b * resy + s.c) / s.a;
        }
        else if (f.b == 0)
        {
            resx = -f.c / f.a;
            resy = -(resx * s.a + s.c) / s.b;
        }
        else
        {
            resy = (f.c * s.a - f.a * s.c) / (f.a * s.b - f.b * s.a);
            resx = (f.c * s.b - s.c * f.b) / (s.a * f.b - f.a * s.b);
        }

        return new Vector2(resx, resy);
    }

    public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
    {
        Vector2 dir = point - pivot;
        dir = Quaternion.Euler(0f, 0f, angle) * dir;
        point = dir + pivot;
        return point;
    }

    public static Vector2 GetCentroid(Vector2 A, Vector2 B, Vector2 C)
    {
        return (A + B + C) / 3f;
    }

    //it's better to use orientated area in the future 
    public static bool IsInsideArbitraryShape(List<Line> segments, Vector2 point)
    {
        if(segments.Count <= 1)
        {
            return false;
        }

        List<Vector2> vertex = new List<Vector2>();

        for (int i = 0; i < segments.Count; i++)
        {
            Line f = segments[i];
            for (int j = i + 1; j < segments.Count; j++)
            {
                Line s = segments[j];
                Vector2 lineIntercetion = LineIntersection(f, s);

                if (IsOnLine(f.First, f.Second, lineIntercetion) &&
                    IsOnLine(s.First, s.Second, lineIntercetion))
                {
                    vertex.Add(lineIntercetion);
                }
            }
        }

        for (int i = 0; i < segments.Count; i++)
        {
            vertex.Add(segments[i].First);
            vertex.Add(segments[i].Second);
        }

        vertex = vertex.OrderBy(x => x.x).ThenBy(x => x.y).ToList();

        List<Vector2> tmp = new List<Vector2>();
        tmp.Add(vertex[0]);
        for (int i = 1; i < vertex.Count; i++)
        {
            if (Compare(vertex[i - 1], vertex[i]))
            {
                continue;
            }
            tmp.Add(vertex[i]);
        }
        vertex = tmp;
        for (int i = 0; i < vertex.Count; i++)
        {
            Vector2 v = vertex[i];
            for (int j = 0; j < segments.Count; j++)
            {
                Vector2 a = segments[j].First;
                Vector2 b = segments[j].Second;

                if (Compare(a, v) || Compare(b, v))
                    continue;
                if (!IsOnLine(a, b, v))
                    continue;
                segments[j] = new Line(a, v);
                segments.Add(new Line(v, b));
            }
        }

        int FindVector2IndexFromVertex(Vector2 vector)
        {
            for(int i = 0; i < vertex.Count; i++)
            {
                if(Compare(vector, vertex[i]))
                    return i;
            }
            throw new System.Exception("Such Vector2 doesn't exist in Vertex.");
        }

        List<List<int>> graph = new List<List<int>>();

        for (int i = 0; i < vertex.Count; i++)
        {
            graph.Add(new List<int>());
        }

        for (int i = 0; i < segments.Count; i++)
        {
            int a = FindVector2IndexFromVertex(segments[i].First);
            int b = FindVector2IndexFromVertex(segments[i].Second);
            graph[a].Add(b);
            graph[b].Add(a);
        }

        List<List<float>> weights = new List<List<float>>();

        for (int i = 0; i < vertex.Count; i++)
        {
            weights.Add(new List<float>());
        }

        for (int i = 0; i < graph.Count; i++)
        {
            Vector2 a = vertex[i];
            for (int j = 0; j < graph[i].Count; j++)
            {
                Vector2 b = vertex[graph[i][j]];
                if (IsOnLine(a, b, point))
                    return true;

                weights[i].Add(AngleABC(a, point, b));
            }
        }

        return FindNonZeroCycle(graph, weights);
    }

    static bool DFS(int s, bool[] used, float[] dist, List<List<int>> graph, List<List<float>> weights)
    {
        used[s] = true;
        for (int i = 0; i < graph[s].Count; i++)
        {
            int u = graph[s][i];
            float weight = weights[s][i];
            if (used[u])
            {
                if (!Compare(dist[u], dist[s] + weight))
                    return true;
            }
            else
            {
                dist[u] = dist[s] + weight;
                if (DFS(u, used, dist, graph, weights))
                    return true;
            }
        }
        return false;
    }

    static bool FindNonZeroCycle(List<List<int>> graph, List<List<float>> weights)
    {
        float[] dist = new float[graph.Count];
        bool[] used = new bool[graph.Count];
        return DFS(0, used, dist, graph, weights);
    }
}
