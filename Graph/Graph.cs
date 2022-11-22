using System.Collections;
using System.Data;

namespace Graph;

public sealed class Graph : IMatrix<bool>, ICloneable
{
    private readonly bool[,]? _matrix;
    public int CountRows { get; private set; }
    public int CountColumn { get; private set; }

    public Graph() => _matrix = null;

    public Graph(bool[,] matrix)
    {
        _matrix = (bool[,])matrix.Clone();

        CountRows = _matrix.GetLength(0);
        CountColumn = _matrix.GetLength(1);
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

        CountColumn = CountRows;

        _matrix = new bool[CountRows, CountColumn];
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
            for (var column = 1; column <= graph.CountColumn; column++)
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

    public static ulong GetCountRoutesBetweenAllTwoVertices(Graph graph, uint lenght)
    {
        if (lenght == 0)
        {
            throw new ArgumentException("lenght can`t equal 0");
        }

        var (_, m) = graph.Exponentiation((int)lenght);

        return m.Cast<ulong>().Aggregate<ulong, ulong>(0, (current, x) => current + x);
    }

    private static void PrintAllRoutesBetweenVertices(Graph graph, IList<int> routes, int endVertex, int
        indexRoute, ulong lenght)
    {
        for (var column = 0; column < graph.CountColumn; column++)
        {
            if (graph[routes[indexRoute - 1] - 1, column])
            {
                routes[indexRoute] = column + 1;
                if (indexRoute + 1 <= Convert.ToInt32(lenght))
                {
                    PrintAllRoutesBetweenVertices(graph, routes, endVertex, indexRoute + 1, lenght);
                }
                else if (routes[indexRoute] == endVertex)
                {
                    Console.WriteLine(string.Join(',', routes.ToString()));
                }
            }
        }
    }

    public static void PrintAllRoutesBetweenVertices(Graph graph, int beginVertex, int endVertex, ulong lenght)
    {
        var routes = new int[lenght + 1];
        routes[0] = (int)(lenght + 1);

        PrintAllRoutesBetweenVertices(graph, routes, endVertex, (int)lenght, lenght);
    }

    public static bool FindEqual<T>(IList<T> iList, T equal, int border)
    {
        for (var i = 0; i <= border; i++)
        {
            if (Equals(iList[i], equal))
            {
                return true;
            }
        }

        return false;
    }

    public static int GetIndexLastEqualZero(IList<int> collection)
    {
        for (var i = 0; i < collection.Count; i++)
        {
            if (collection[i] == 0)
            {
                return i;
            }

            if (i + 1 == collection.Count)
            {
                return i + 1;
            }
        }

        return collection.Count;
    }

    public static bool AllAreUnique(IList<int> collection, int lastElement)
    {
        for (var i = 0; i < lastElement - 1; i++)
        {
            for (var j = i + 1; j < lastElement; j++)
            {
                if (collection[i] == collection[j + 1])
                {
                    return false;
                }
            }
        }

        return false;
    }

    public static bool IsInclude(Graph graph, IList<int> collection)
    {
        for (var i = 0; i < collection.Count - 1 && collection[i] != 0; i++)
        {
            if (!graph[collection[i] - 1, collection[i + 1] - 1])
            {
                return false;
            }
        }

        return true;
    }

    private static void PrintAllMaxSimpleChain(Graph graph, IList<int> routes, int indexRoute)
    {
        var f = true;
        for (var column = 0; column < graph.CountColumn; column++)
        {
            if (graph[routes[indexRoute - 1] - 1, column] && !FindEqual(routes, column + 1, indexRoute - 1))
            {
                f = false;
                routes[indexRoute] = column + 1;
                PrintAllMaxSimpleChain(graph, routes, indexRoute + 1);
            }

            if (f && IsInclude(graph, routes) && AllAreUnique(routes, GetIndexLastEqualZero(routes)))
            {
                Console.WriteLine(string.Join(',', routes.ToString()));
            }
        }
    }

    public static void PrintAllMaxSimpleChain(Graph graph, int vertex)
    {
        var routes = new int[graph.CountRows - 1];
        routes[0] = vertex;

        PrintAllMaxSimpleChain(graph, routes, 1);
    }

    public object Clone() => new Graph(CountRows, CountRows);

    public IEnumerator<bool> GetEnumerator() =>
        _matrix?.Cast<bool>().GetEnumerator() ?? throw new NullReferenceException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}