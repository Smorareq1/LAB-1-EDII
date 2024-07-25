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

    // Implement DELETE operation
    public void Delete(string name)
    {
        Delete(root, name);
        if (root.Keys.Count == 0)
        {
            if (!root.IsLeaf)
            {
                root = root.Children[0];
            }
        }
    }

    private void Delete(BNodo node, string name)
    {
        int idx = node.Keys.FindIndex(k => k == name);

        if (idx != -1)
        {
            if (node.IsLeaf)
            {
                node.Keys.RemoveAt(idx);
                node.Values.RemoveAt(idx);
            }
            else
            {
                BNodo predecessorNode = node.Children[idx];
                if (predecessorNode.Keys.Count >= t)
                {
                    var predecessor = GetPredecessor(predecessorNode);
                    node.Keys[idx] = predecessor.Name;
                    node.Values[idx] = predecessor;
                    Delete(predecessorNode, predecessor.Name);
                }
                else
                {
                    BNodo successorNode = node.Children[idx + 1];
                    if (successorNode.Keys.Count >= t)
                    {
                        var successor = GetSuccessor(successorNode);
                        node.Keys[idx] = successor.Name;
                        node.Values[idx] = successor;
                        Delete(successorNode, successor.Name);
                    }
                    else
                    {
                        Merge(node, idx);
                        Delete(predecessorNode, name);
                    }
                }
            }
        }
        else
        {
            int i = 0;
            while (i < node.Keys.Count && string.Compare(name, node.Keys[i]) > 0)
            {
                i++;
            }
            if (node.Children[i].Keys.Count < t)
            {
                if (i > 0 && node.Children[i - 1].Keys.Count >= t)
                {
                    BorrowFromPrev(node, i);
                }
                else if (i < node.Keys.Count && node.Children[i + 1].Keys.Count >= t)
                {
                    BorrowFromNext(node, i);
                }
                else
                {
                    if (i < node.Keys.Count)
                    {
                        Merge(node, i);
                    }
                    else
                    {
                        Merge(node, i - 1);
                    }
                }
            }
            Delete(node.Children[i], name);
        }
    }

    private Book GetPredecessor(BNodo node)
    {
        while (!node.IsLeaf)
        {
            node = node.Children[node.Keys.Count];
        }
        return node.Values[node.Values.Count - 1];
    }

    private Book GetSuccessor(BNodo node)
    {
        while (!node.IsLeaf)
        {
            node = node.Children[0];
        }
        return node.Values[0];
    }

    private void Merge(BNodo node, int idx) 
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx + 1];

        child.Keys.Add(node.Keys[idx]);
        child.Keys.AddRange(sibling.Keys);
        if (!child.IsLeaf)
        {
            child.Children.AddRange(sibling.Children);
        }
        else
        {
            child.Values.AddRange(sibling.Values);
            child.Next = sibling.Next;
        }

        node.Keys.RemoveAt(idx);
        node.Children.RemoveAt(idx + 1);
    }

    private void BorrowFromPrev(BNodo node, int idx)
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx - 1];

        child.Keys.Insert(0, node.Keys[idx - 1]);
        if (!child.IsLeaf)
        {
            child.Children.Insert(0, sibling.Children[sibling.Children.Count - 1]);
            sibling.Children.RemoveAt(sibling.Children.Count - 1);
        }
        else
        {
            child.Values.Insert(0, sibling.Values[sibling.Values.Count - 1]);
            sibling.Values.RemoveAt(sibling.Values.Count - 1);
        }
        node.Keys[idx - 1] = sibling.Keys[sibling.Keys.Count - 1];
        sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
    }

    private void BorrowFromNext(BNodo node, int idx)
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx + 1];

        child.Keys.Add(node.Keys[idx]);
        if (!child.IsLeaf)
        {
            child.Children.Add(sibling.Children[0]);
            sibling.Children.RemoveAt(0);
        }
        else
        {
            child.Values.Add(sibling.Values[0]);
            sibling.Values.RemoveAt(0);
        }
        node.Keys[idx] = sibling.Keys[0];
        sibling.Keys.RemoveAt(0);
    }


    // Implement PATCH operation
    public void Patch(string name, Dictionary<string, string> updates)
    {
        // Implementation of B+-Tree patch
    }

    // Implement SEARCH operation
    public List<Book> SearchByName(string name)
    {
        // Implementation of B+-Tree search
        return new List<Book>();
    }
}