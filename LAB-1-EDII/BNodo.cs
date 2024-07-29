namespace LAB_1_EDII;

public class BNodo
{
    public List<Book> Books { get; private set; }
    public List<BNodo> Children { get; private set; }
    public bool IsLeaf => Children.Count == 0;
    public int Degree { get; private set; }

    public BNodo(int degree)
    {
        Degree = degree;
        Books = new List<Book>();
        Children = new List<BNodo>();
    }
}
