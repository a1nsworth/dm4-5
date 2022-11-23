namespace Graph;

public class Program
{
    public static void PrintPropertiesVertices(Graph g, IEnumerable<IList<uint>> collectionVertices)
    {
        if (collectionVertices == null) throw new ArgumentNullException(nameof(collectionVertices));

        foreach (var vertices in collectionVertices)
        {
            Console.Write(string.Join(',', vertices.Select(x => x)));
            Console.Write(" : ");
            Console.Write(g.IsRoute(vertices) ? "Маршрут" : "Не маршрут");
            Console.Write(", ");
            Console.Write(g.IsChain(vertices) ? "Цепь" : "Не цепь");
            Console.Write(", ");
            Console.Write(g.IsSimpleChain(vertices) ? "Простая цепь" : "Не простая цепь");
            Console.Write(", ");
            Console.Write(g.IsCycle(vertices) ? "Цикл" : "Не цикл");
            Console.Write(",");
            Console.Write(g.IsSimpleCycle(vertices) ? "Простой цикл" : "Не простой цикл");

            Console.WriteLine();
        }
    }

    public static void Main()
    {
        var vertices = new List<List<uint>>(5)
        {
            new() { 6, 7, 1, 4, 3, 2 },
            new() { 2, 1, 7, 6, 1, 4 },
            new() { 1, 2, 3, 4, 1 },
            new() { 1, 2, 3, 4, 2, 1 },
            new() { 2, 1, 6, 7, 1, 4, 2 }
        };

        var graphs = new List<Graph>
        {
            new((1, 2), (2, 1), (1, 4), (4, 1), (1, 5), (5, 1), (1, 6), (6, 1), (1, 7), (7, 1),
                (2, 3), (3, 2), (2, 5), (5, 2), (2, 6), (6, 2),
                (3, 4), (4, 3),
                (6, 7), (7, 6)),
            new((1, 2), (2, 1), (1, 4), (4, 1), (1, 6), (6, 1), (1, 7), (7, 1),
                (2, 3), (3, 2), (2, 4), (4, 2),
                (3, 6), (6, 3), (3, 5), (5, 3), (3, 4), (4, 3),
                (4, 5), (5, 4),
                (6, 7), (7, 6))
        };

        Graph.PrintAllMaxSimpleChain(graphs[0]);
        Console.WriteLine();
        Graph.PrintAllMaxSimpleChain(graphs[1]);
    }
}