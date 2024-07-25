namespace LAB_1_EDII;

public class BPlusTree
{
    private int t; // Minimum degree
    private BNodo root;

    public BPlusTree(int t)
    {
        this.t = t;
        root = new BNodo(true);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Implement INSERT operation
    public void Insert(Book book)
    {
        if (root.Keys.Count == 2 * t - 1)
        {
            var newRoot = new BNodo(false);
            newRoot.Children.Add(root);
            SplitChild(newRoot, 0, root);
            root = newRoot;
        }
        InsertNonFull(root, book);
    }

    private void SplitChild(BNodo parent, int i, BNodo fullChild)
    {
        var newChild = new BNodo(fullChild.IsLeaf);
        parent.Children.Insert(i + 1, newChild);
        parent.Keys.Insert(i, fullChild.Keys[t - 1]);

        newChild.Keys.AddRange(fullChild.Keys.GetRange(t, t - 1));
        fullChild.Keys.RemoveRange(t - 1, t);

        if (!fullChild.IsLeaf)
        {
            newChild.Children.AddRange(fullChild.Children.GetRange(t, t));
            fullChild.Children.RemoveRange(t, t);
        }
        else
        {
            newChild.Values.AddRange(fullChild.Values.GetRange(t, t - 1));
            fullChild.Values.RemoveRange(t, t - 1);
            newChild.Next = fullChild.Next;
            fullChild.Next = newChild;
        }
    }

    private void InsertNonFull(BNodo node, Book book)
    {
        if (node.IsLeaf) 
        {
            int i = node.Keys.Count - 1;
            while (i >= 0 && string.Compare(node.Keys[i], book.Name) > 0)
            {
                i--;
            }
            node.Keys.Insert(i + 1, book.Name);
            node.Values.Insert(i + 1, book);
        }
        else
        {
            int i = node.Keys.Count - 1;
            while (i >= 0 && string.Compare(node.Keys[i], book.Name) > 0)
            {
                i--;
            }
            i++;
            if (node.Children[i].Keys.Count == 2 * t - 1)
            {
                SplitChild(node, i, node.Children[i]);
                if (string.Compare(book.Name, node.Keys[i]) > 0)
                {
                    i++;
                }
            }
            InsertNonFull(node.Children[i], book);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // BORRAR
    public void Delete(string isbn)
    {
        if (root == null)
        {
            Console.WriteLine("El árbol está vacío.");
            return;
        }

        Delete(root, isbn);

        if (root.Keys.Count == 0)
        {
            if (root.IsLeaf)
            {
                root = null;
            }
            else
            {
                root = root.Children[0];
            }
        }
    }


    private void Delete(BNodo node, string isbn)
    {
        var (foundNode, idx) = SearchByIdIndex(node, isbn);

        if (foundNode == null)
        {
            Console.WriteLine($"El ISBN {isbn} no está en el árbol.");
            return;
        }

        Console.WriteLine($"Delete: ISBN = {isbn}, Index = {idx}, Node Keys = {string.Join(", ", foundNode.Keys)}");

        if (foundNode.IsLeaf)
        {
            foundNode.Keys.RemoveAt(idx);
            foundNode.Values.RemoveAt(idx);
        }
        else
        {
            DeleteFromNonLeaf(foundNode, idx);
        }

        if (root.Keys.Count == 0)
        {
            if (root.IsLeaf)
            {
                root = null;
            }
            else
            {
                root = root.Children[0];
            }
        }
    }

    
    private (BNodo, int) SearchByIdIndex(BNodo node, string isbn)
    {
        if (node == null)
        {
            return (null, -1);
        }

        for (int i = 0; i < node.Values.Count; i++)
        {
            if (node.Values[i].Isbn == isbn)
            {
                return (node, i);
            }
        }

        foreach (var child in node.Children)
        {
            var result = SearchByIdIndex(child, isbn);
            if (result.Item1 != null)
            {
                return result;
            }
        }

        return (null, -1);
    }



    private void DeleteFromNonLeaf(BNodo node, int idx)
    {
        string k = node.Keys[idx];

        if (node.Children[idx].Keys.Count >= t)
        {
            string pred = GetPredecessor(node, idx);
            node.Keys[idx] = pred;
            Delete(node.Children[idx], pred);
        }
        else if (node.Children[idx + 1].Keys.Count >= t)
        {
            string succ = GetSuccessor(node, idx);
            node.Keys[idx] = succ;
            Delete(node.Children[idx + 1], succ);
        }
        else
        {
            Merge(node, idx);
            Delete(node.Children[idx], k);
        }
    }
    
    private string GetPredecessor(BNodo node, int idx)
    {
        BNodo cur = node.Children[idx];
        while (!cur.IsLeaf)
        {
            cur = cur.Children[cur.Keys.Count];
        }
        return cur.Keys[cur.Keys.Count - 1];
    }
    
    private string GetSuccessor(BNodo node, int idx)
    {
        BNodo cur = node.Children[idx + 1];
        while (!cur.IsLeaf)
        {
            cur = cur.Children[0];
        }
        return cur.Keys[0];
    }
    
    private void Merge(BNodo node, int idx)
    {
        BNodo child = node.Children[idx];
        BNodo sibling = node.Children[idx + 1];

        child.Keys.Insert(t - 1, node.Keys[idx]);

        for (int i = 0; i < sibling.Keys.Count; ++i)
        {
            child.Keys.Insert(i + t, sibling.Keys[i]);
        }

        if (!child.IsLeaf)
        {
            for (int i = 0; i <= sibling.Keys.Count; ++i)
            {
                child.Children.Insert(i + t, sibling.Children[i]);
            }
        }

        for (int i = idx + 1; i < node.Keys.Count; ++i)
        {
            node.Keys[i - 1] = node.Keys[i];
        }

        for (int i = idx + 2; i <= node.Children.Count; ++i)
        {
            node.Children[i - 1] = node.Children[i];
        }

        node.Keys.RemoveAt(node.Keys.Count - 1);
        node.Children.RemoveAt(node.Children.Count - 1);
    }



   

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // PATCH
    public void Patch(string isbn, Dictionary<string, string> updates)
    {
        var book = SearchById(root, isbn);
        if (book != null)
        {
            foreach (var key in updates.Keys)
            {
                switch (key)
                {
                    case "Author":
                        book.Author = updates[key];
                        break;
                    case "Category":
                        book.Category = updates[key];
                        break;
                    case "Price":
                        book.Price = updates[key];
                        break;
                    case "Quantity":
                        book.Quantity = updates[key];
                        break;
                }
            }
        }
    }

    private Book SearchById(BNodo node, string isbn)
    {
        if (node == null)
        {
            return null;
        }
        foreach (var book in node.Values)
        {
            if (book.Isbn == isbn)
            {
                return book;
            }
        }
        foreach (var child in node.Children)
        {
            var result = SearchById(child, isbn);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // IBUSCAR POR NOMBRE
    public List<Book> SearchByName(string name)
    {
        var result = new List<Book>();
        SearchByNameRecursive(root, name, result);
        
        return result;
    }

    private void SearchByNameRecursive(BNodo node, string name, List<Book> result)
    {
        if (node == null)
        {
            return;
        }
        for (int i = 0; i < node.Keys.Count; i++)
        {
            if (node.Keys[i].Contains(name))
            {
                result.Add(node.Values[i]);
            }
        }
        foreach (var child in node.Children)
        {
            SearchByNameRecursive(child, name, result);
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void PrintSearchResults(List<Book> books)
    {
        foreach (var book in books)
        {
            Console.WriteLine($"ISBN: {book.Isbn}, Name: {book.Name}, Author: {book.Author}, Category: {book.Category}, Price: {book.Price}, Quantity: {book.Quantity}");
        }

        if (books.Count == 0)
        {
            Console.WriteLine("No se encontro el libro");
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void PrintTree()
    {
        PrintNode(root, 0);
    }

    private void PrintNode(BNodo node, int level)
    {
        Console.WriteLine(new string(' ', level * 2) + string.Join(", ", node.Keys));

        if (!node.IsLeaf)
        {
            foreach (var child in node.Children)
            {
                PrintNode(child, level + 1);
            }
        }
    }


}