using System.Diagnostics.SymbolStore;

namespace Graph;

public static class VerticesPropertiesGraphExtension
{
    public static bool IsCycleRoute(IList<uint> vertices) =>
        Equals(vertices[0], vertices[^1]);

    public static bool IsRoute(this Graph graph, IList<uint> vertices)
    {
        if (graph.GetData() is null)
        {
            throw new NullReferenceException("_matrix is null");
        }

        for (var i = 1; i < vertices.Count; i++)
        {
            if (!graph[(int)(vertices[i] - 1), (int)(vertices[i - 1] - 1)])
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsChain(this Graph graph, IList<uint> vertices)
    {
        if (!IsRoute(graph, vertices))
            return false;

        var edges = new List<Tuple<uint, uint>>();
        for (var i = 0; i < vertices.Count - 1; i++)
        {
            edges.Add(new Tuple<uint, uint>(vertices[i], vertices[i + 1]));
        }

        for (var i = 0; i < edges.Count - 1; i++)
        {
            for (var j = i + 1; j < edges.Count; j++)
            {
                var reverseTuple = new Tuple<uint, uint>(edges[j].Item2, edges[j].Item1);
                if (Equals(edges[i], edges[j]) || Equals(edges[i], reverseTuple))
                    return false;
            }
        }

        return true;
    }

    public static bool IsSimpleChain(this Graph graph, IList<uint> vertices)
    {
        if (!IsChain(graph, vertices))
            return false;

        for (var i = 0; i < vertices.Count - 1; i++)
        {
            for (var j = i + 1; j < vertices.Count; j++)
            {
                if ((i != 0 || j != vertices.Count - 1) &&
                    vertices[i] == vertices[j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsCycle(this Graph graph, IList<uint> vertices) =>
        IsCycleRoute(vertices) && IsChain(graph, vertices);

    public static bool IsSimpleCycle(this Graph graph, IList<uint> vertices) =>
        IsCycleRoute(vertices) && IsSimpleChain(graph, vertices);
}