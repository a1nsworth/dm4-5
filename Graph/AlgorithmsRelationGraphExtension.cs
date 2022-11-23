namespace Graph;

public static class AlgorithmsRelationGraphExtension
{
    public static ulong[,] Composition(this Graph graph1, ulong[,] graph2)
    {
        var result = new ulong[graph1.CountRows, graph1.CountColumn];

        for (var row = 0; row < graph1.CountRows; row++)
        {
            for (var column = 0; column < graph1.CountColumn; column++)
            {
                for (var z = 0; z < graph1.CountColumn; z++)
                {
                    result[row, column] += Convert.ToUInt32(graph1[row, z]) * graph2[z, column];
                }
            }
        }

        return result;
    }

    public static ulong[,] Exponentiation(this Graph graph, int degree)
    {
        var result = new ulong[graph.CountRows, graph.CountColumn];
        for (var row = 0; row < graph.CountRows; row++)
        {
            for (var column = 0; column < graph.CountColumn; column++)
            {
                if (graph[row, column])
                    result[row, column] = 1;
            }
        }

        switch (degree)
        {
            case 0:
            {
                for (var row = 0; row < graph.CountRows; row++)
                {
                    for (var column = 0; column < graph.CountColumn; column++)
                    {
                        result[row, column] = 1;
                    }
                }

                break;
            }
            case > 1:
            {
                for (var i = 2; i <= degree; i++)
                {
                    result = Composition(graph, result);
                }

                break;
            }
        }

        return result;
    }
}