namespace Graph;

public struct BenchmarkInfo
{
    public int CountEulerGraphs { get; set; } = 0;
    public int CountHamiltonianGraphs { get; set; } = 0;
    public int TotalGeneratedGraphs { get; set; } = 0;

    public BenchmarkInfo(int countEulerGraphs, int countHamiltonianGraphs)
    {
        CountEulerGraphs = countEulerGraphs;
        CountHamiltonianGraphs = countHamiltonianGraphs;
        TotalGeneratedGraphs = CountEulerGraphs + CountHamiltonianGraphs;
    }

    public BenchmarkInfo() : this(0, 0)
    {
    }

    public void Reset()
    {
        CountEulerGraphs = 0;
        CountHamiltonianGraphs = 0;
        TotalGeneratedGraphs = 0;
    }

    public override string ToString() =>
        $"Всего графов: {TotalGeneratedGraphs}\n" +
        $"Эйлеровых графов: {CountEulerGraphs}\n" +
        $"Гамильтоновых графов: {CountEulerGraphs}";
}