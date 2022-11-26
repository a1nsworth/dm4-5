using System.Diagnostics;
using System.Threading.Channels;

namespace Graph;

public class Program
{
    public static void Main()
    {
        const uint nVertices = 8;
        const uint maxEdges = (nVertices * (nVertices - 1)) / 2;
        // for (uint i = 0, nEdges = nVertices; nEdges < maxEdges; i++, nEdges = nVertices + i * nEdges)
        // {
        //     Console.WriteLine(BenchmarkGraphExtension.GetBenchmarkInfo(nVertices, nEdges, 100));
        // }

        // var g = new Graph((1, 2), (2, 1), (2, 5), (5, 2), (2, 4), (4, 2), (2, 3), (3, 2), (3, 4), (4, 3), (5, 4),
        //     (4, 5)); 
        // var g = new Graph((1, 2), (2, 1), (1,3),(3,1),(4,5),(5,4),(4,3),(3,4),(2,5),(5,2)); 
        var g = new Graph((1, 2), (2, 1), (1, 5), (5, 1), (2, 4), (4, 2), (2, 5), (5, 2), (4, 3), (3, 4), (5, 4),
            (4, 5));
        Console.WriteLine (string.Join(", ", Graph.GetHamiltonianCycle(g).Select(x => x)));
        //
        // if (Graph.IsEuler(g))
        // {
        //     Console.WriteLine("IsEuler");
        // }
        //
        // if (Graph.IsHamiltonian(g))
        // {
        //     Console.WriteLine("IsHamiltonian");
        // }
    }
}