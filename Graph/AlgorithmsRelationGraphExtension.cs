namespace Graph;

public static class AlgorithmsRelationGraphExtension
{
    public static Graph Composition(this Graph graph1, Graph graph2, ref ulong[,] matrix)
    {
        var list = new List<(int, int)>();
        var minLength = Math.Min(Math.Min(graph1.CountColumn, graph2.CountColumn),
            Math.Min(graph1.CountRows, graph2.CountRows));
        var maxElement = 0;
        for (var row = 1; row <= minLength; row++)
        {
            for (var column = 1; column <= minLength; column++)
            {
                for (var z = 0; z < minLength; z++)
                {
                    if (graph1[row - 1, z] && graph2[z, column - 1])
                    {
                        maxElement = Math.Max(maxElement, Math.Max(row, column));
                        list.Add((row, column));

                        matrix[row - 1, column - 1] += Convert.ToUInt32(graph1[row - 1, z - 1]) *
                                                       Convert.ToUInt32(graph2[z - 1, column - 1]);
                    }
                }
            }
        }

        return maxElement == 0
            ? new Graph(Math.Min(graph1.CountRows, graph2.CountRows), Math.Min(graph1.CountColumn, graph2.CountColumn))
            : new Graph(list, maxElement);
    }

    public static (Graph, ulong[,]) Exponentiation(this Graph graph, int degree)
    {
        var result = new Graph(graph.CountRows, graph.CountColumn);
        var size = Math.Min(result.CountColumn, result.CountRows);
        var m = new ulong[graph.CountRows, graph.CountColumn];

        if (degree == 0)
        {
            for (var row = 0; row < size; row++)
            {
                result[row, row] = true;
                m[row, row] += Convert.ToUInt32(result[row, row]);
            }
        }
        else
        {
            result = (Graph)graph.Clone();
            for (var i = 2; i <= degree; i++)
            {
                result = Composition(graph, result, ref m);
            }
        }

        return (result, m);
    }
    
    
}