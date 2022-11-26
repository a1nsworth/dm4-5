using System.Diagnostics;

namespace Graph;

public static class BenchmarkGraphExtension
{
    public static Graph GenerateRandomGraph(ulong nVertices, ulong nEdges)
    {
        var graph = new Graph((int)nVertices, (int)nVertices);
        ulong currentEdges = 0;
        while (currentEdges < nEdges)
        {
            var (row, column) = (new Random().Next(0, graph.CountRows), new Random().Next(0, graph.CountRows));

            if (row != column && !graph[row, column])
            {
                graph[row, column] = true;
                currentEdges++;
            }
        }

        return graph;
    }

    public static List<Graph> GenerateRandomGraphsByTime(ulong nVertices, ulong nEdges, ulong rangeInMilliSeconds)
    {
        var graphs = new List<Graph>();

        var timeLeft = new Stopwatch();
        timeLeft.Start();
        while (timeLeft.ElapsedMilliseconds < (long)rangeInMilliSeconds)
        {
            var graph = GenerateRandomGraph(nVertices, nEdges);

            graphs.Add(graph);
        }

        timeLeft.Stop();

        return graphs;
    }

    public static BenchmarkInfo GetBenchmarkInfo(ulong nVertices, ulong nEdges, ulong rangeInMilliSeconds = 10000)
    {
        var generatedGraphs = GenerateRandomGraphsByTime(nVertices, nEdges, rangeInMilliSeconds);
        var benchmarkInfo = new BenchmarkInfo
        {
            TotalGeneratedGraphs = generatedGraphs.Count
        };

        generatedGraphs.ForEach(graph =>
        {
            if (Graph.IsEuler(graph))
            {
                benchmarkInfo.CountEulerGraphs++;
            }
            else if (Graph.IsHamiltonian(graph))
            {
                benchmarkInfo.CountHamiltonianGraphs++;
            }
        });

        return benchmarkInfo;
    }
}