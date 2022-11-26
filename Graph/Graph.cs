using System.Collections;
using System.Data;

namespace Graph;

public sealed class Graph : IMatrix<bool>, ICloneable
{
    private readonly bool[,]? _matrix;
    public int CountRows { get; private set; }
    public int CountColumns { get; private set; }

    public Graph() => _matrix = null;

    public Graph(bool[,] matrix)
    {
        _matrix = (bool[,])matrix.Clone();

        CountRows = _matrix.GetLength(0);
        CountColumns = _matrix.GetLength(1);
    }

    public Graph(int nRows, int nColumns) : this(new bool[nRows, nColumns])
    {
    }

    public Graph(IEnumerable<(int, int)> enumerable, int nRows, int nColumns) : this(nRows, nColumns)
    {
        if (_matrix is null)
            throw new NullReferenceException();

        foreach (var pair in enumerable)
            _matrix[pair.Item1 - 1, pair.Item2 - 1] = true;
    }

    public Graph(IEnumerable<(int, int)> enumerable, int n) : this(n, n)
    {
        if (_matrix is null)
            throw new NullReferenceException();

        foreach (var pair in enumerable)
            _matrix[pair.Item1 - 1, pair.Item2 - 1] = true;
    }

    public Graph(IEnumerable<(int, int)> enumerable) : this()
    {
        var values = new List<(int, int)>();
        foreach (var tuple in enumerable)
        {
            values.Add(tuple);
            CountRows = Math.Max(CountRows, Math.Max(tuple.Item1, tuple.Item2));
        }

        CountColumns = CountRows;

        _matrix = new bool[CountRows, CountColumns];
        foreach (var tuple in values)
            _matrix[tuple.Item1 - 1, tuple.Item2 - 1] = true;
    }

    public Graph(in Graph graph) : this(graph._matrix ?? throw new NullReferenceException())
    {
    }

    public Graph(params (int, int)[] valuesTuples) : this((IEnumerable<(int, int)>)valuesTuples)
    {
    }

    public bool[,]? GetData() => _matrix;

    public bool this[int iRow, int iColumn]
    {
        get => _matrix?[iRow, iColumn] ?? throw new NullReferenceException();
        set
        {
            if (_matrix is null)
                throw new NullReferenceException();
            _matrix[iRow, iColumn] = value;
        }
    }


    private static void GetListVerticesByLenght(Graph graph, uint beginVertex, uint length, List<uint> selectedVertices,
        ref List<List<uint>> resultVertices)
    {
        if (selectedVertices.Count == length)
        {
            resultVertices.Add(selectedVertices.GetRange(0, selectedVertices.Count));
            selectedVertices.RemoveAt(selectedVertices.Count - 1);
        }
        else
        {
            for (var column = 1; column <= graph.CountColumns; column++)
            {
                if (graph[(int)beginVertex - 1, column - 1])
                {
                    selectedVertices.Add((uint)column);

                    GetListVerticesByLenght(graph, (uint)column, length,
                        selectedVertices, ref resultVertices);
                }
            }

            selectedVertices.RemoveAt(selectedVertices.Count - 1);
        }
    }

    public static List<List<uint>> GetListVerticesByLenght(Graph graph, uint beginVertex, uint length)
    {
        var resultVertices = new List<List<uint>>();
        var selectedVertices = new List<uint> { beginVertex };
        GetListVerticesByLenght(graph, beginVertex, length + 1, selectedVertices, ref resultVertices);

        return resultVertices;
    }

    public static ulong GetCountRoutesBetweenAllTwoVertices(Graph graph, int lenght)
    {
        var r = new ulong[graph.CountRows, graph.CountColumns];
        for (var row = 0; row < graph.CountRows; row++)
        {
            for (var column = 0; column < graph.CountColumns; column++)
            {
                if (graph[row, column])
                    r[row, column] = 1;
            }
        }

        r = graph.Exponentiation(lenght);

        ulong count = 0;
        for (var row = 0; row < graph.CountRows; row++)
        {
            for (var columns = 0; columns < graph.CountColumns; columns++)
            {
                if (r[row, columns] != 0)
                    count += r[row, columns];
            }
        }

        return count;
    }

    private static void MakeAllRoutesBetweenVertices(Graph graph, IList<uint> routes, ref List<IList<uint>> allRoutes,
        int endVertex, int
            indexRoute, ulong lenght)
    {
        for (var column = 0; column < graph.CountColumns; column++)
        {
            if (graph[(int)(routes[indexRoute - 1] - 1), column])
            {
                routes[indexRoute] = (uint)(column + 1);
                if (indexRoute + 1 <= Convert.ToInt32(lenght))
                {
                    MakeAllRoutesBetweenVertices(graph, routes, ref allRoutes, endVertex, indexRoute + 1, lenght);
                }
                else if (routes[indexRoute] == endVertex)
                {
                    allRoutes.Add(new List<uint>(routes));
                }
            }
        }
    }

    public static List<IList<uint>> GetAllRoutesBetweenVertices(Graph graph, int beginVertex, int endVertex,
        ulong lenght)
    {
        var routes = new uint[lenght + 1];
        routes[0] = (uint)beginVertex;

        var allRoutes = new List<IList<uint>>();
        MakeAllRoutesBetweenVertices(graph, routes, ref allRoutes, endVertex, 1, lenght);

        return allRoutes;
    }

    public static bool FindEqual<T>(IList<T> collection, T equal, int border)
    {
        for (var i = 0; i < border; i++)
        {
            if (Equals(collection[i], equal))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsInclude(Graph graph, IList<int> collection)
    {
        for (var i = 0; i < collection.Count - 1; i++)
        {
            if (!graph[collection[i] - 1, collection[i + 1] - 1])
            {
                return false;
            }
        }

        return true;
    }

    private static void PrintAllMaxSimpleChain(Graph graph, List<int> routes, int indexRoute)
    {
        var isUnique = true;
        for (var column = 0; column < graph.CountColumns && indexRoute < routes.Count; column++)
        {
            if (graph[routes[indexRoute - 1] - 1, column] && !FindEqual(routes, column + 1, indexRoute))
            {
                isUnique = false;
                routes[indexRoute] = column + 1;
                PrintAllMaxSimpleChain(graph, routes, indexRoute + 1);
            }
        }

        if (isUnique)
        {
            while (routes[^1] == 0)
            {
                routes.RemoveAt(routes.Count - 1);
            }

            if (IsInclude(graph, routes))
            {
                Console.WriteLine(string.Join(',', routes.Select(x => x)));
            }
        }
    }

    public static void PrintAllMaxSimpleChain(Graph graph)
    {
        var routes = Enumerable.Repeat(0, graph.CountRows).ToList();

        for (var i = 1; i <= graph.CountRows; i++)
        {
            routes[0] = i;
            PrintAllMaxSimpleChain(graph, routes, 1);
        }
    }

    public static bool IsContainAllUniqueVertices(Graph graph, IList<uint> vertices)
    {
        for (uint vertex = 1; vertex <= graph.CountRows; vertex++)
        {
            if (vertices.Count(x => x == vertex) != 1)
            {
                return false;
            }
        }

        return true;
    }

    public static List<(uint, uint)> GetAllEdges(Graph graph)
    {
        var edges = new List<(uint, uint)>();
        for (uint row = 1; row <= graph.CountRows; row++)
        {
            for (uint column = 1; column <= graph.CountColumns; column++)
            {
                if (graph[(int)(row - 1), (int)(column - 1)])
                {
                    edges.Add((row, column));
                }
            }
        }

        return edges;
    }

    public static IList<(uint, uint)> GetUniqueEdges(List<(uint, uint)> edges)
    {
        var uniqueEdges = new List<(uint, uint)>(edges.Count) { edges[0] };
        edges.ForEach(edge =>
        {
            var reverseEdge = (edge.Item2, edge.Item1);
            if (!uniqueEdges.Contains(edge) && !uniqueEdges.Contains(reverseEdge))
            {
                uniqueEdges.Add(edge);
            }
        });

        return uniqueEdges;
    }

    private static List<(T, T)> CreateTuplesFromCollection<T>(IList<T> collection)
    {
        var tuples = new List<(T, T)>();
        for (var i = 1; i < collection.Count; i++)
        {
            tuples.Add((collection[i - 1], collection[i]));
        }

        return tuples;
    }

    public static bool IsContainAllEdges(List<(uint, uint)> edges, List<uint> route)
    {
        var tuples = CreateTuplesFromCollection(route);
        foreach (var edge in edges)
        {
            var isFind = false;
            foreach (var tuple in tuples)
            {
                var reverseTuple = (tuple.Item2, tuple.Item1);
                if (Equals(edge, tuple) || Equals(edge, reverseTuple))
                {
                    if (isFind)
                        return false;

                    isFind = true;
                }
            }
        }

        return true;
    }

    public static bool IsEuler(Graph graph)
    {
        var uniqueEdges = GetUniqueEdges(GetAllEdges(graph));

        for (var beginVertex = 1; beginVertex < graph.CountRows; beginVertex++)
        {
            for (var endVertex = 1; endVertex < graph.CountRows; endVertex++)
            {
                var routes = GetAllRoutesBetweenVertices(graph, beginVertex, endVertex, (ulong)uniqueEdges.Count);
                if (routes.Any(route =>
                        graph.IsSimpleCycle((List<uint>)route) &&
                        IsContainAllEdges((List<(uint, uint)>)uniqueEdges, (List<uint>)route)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsHamiltonian(Graph graph)
    {
        for (var lenght = 2; lenght < graph.CountRows; lenght++)
        {
            for (var beginVertex = 1; beginVertex < graph.CountRows; beginVertex++)
            {
                for (var endVertex = 1; endVertex < graph.CountRows; endVertex++)
                {
                    var routes =
                        GetAllRoutesBetweenVertices(graph, beginVertex, endVertex, (ulong)(graph.CountRows - 1));

                    if (routes.Any(route => IsContainAllUniqueVertices(graph, route)))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool GetHamiltonianCycle(Graph graph, ref List<uint> vertices, ref IList<bool> visited,
        uint element)
    {
        vertices.Add(element);

        if (vertices.Count == graph.CountRows)
        {
            if (graph[(int)vertices[0], (int)vertices[^1]])
            {
                return true;
            }

            vertices.RemoveAt(vertices.Count - 1);
            return false;
        }

        visited[(int)element] = true;
        for (int vertex = 0; vertex < graph.CountRows; vertex++)
        {
            if (graph[(int)element, vertex] && !visited[vertex])
            {
                if (GetHamiltonianCycle(graph, ref vertices, ref visited, (uint)vertex))
                {
                    return true;
                }
            }
        }

        visited[(int)element] = false;
        vertices.RemoveAt(vertices.Count - 1);

        return false;
    }

    public static dynamic GetHamiltonianCycle(Graph graph)
    {
    }

    public object Clone() => new Graph(CountRows, CountColumns);

    public IEnumerator<bool> GetEnumerator() =>
        _matrix?.Cast<bool>().GetEnumerator() ?? throw new NullReferenceException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}