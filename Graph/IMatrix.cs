namespace Graph;

public interface IMatrix<T> : IEnumerable<T>
{
    public int CountRows { get; }
    public int CountColumns { get; }
    public T this[int iRow, int iColumn] { get; set; }
}