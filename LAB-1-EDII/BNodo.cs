namespace LAB_1_EDII;

public class BNodo
{
    public List<string> Keys { get; set; } = new List<string>();
    public List<BNodo> Children { get; set; } = new List<BNodo>();
    public List<Book> Values { get; set; } = new List<Book>();
    public BNodo Next { get; set; }
    public bool IsLeaf { get; set; } = true;

    public BNodo(bool isLeaf)
    {
        IsLeaf = isLeaf;
    }
}