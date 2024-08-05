
namespace LAB_1_EDII;
public class BPlusTree
    {
       private readonly int _degree;
        private readonly int _minKeys;
        private readonly int _maxKeys;
        private BNodo _root;

        public BPlusTree(int degree)
        {
            _degree = degree;
            _minKeys = (degree + 1) / 2 - 1;
            _maxKeys = degree - 1;
            _root = new BNodo(degree);
        }

        public void Insert(Book book)
        {
            var root = _root;
            if (root.Books.Count == _maxKeys + 1)
            {
                var newRoot = new BNodo(_degree);
                newRoot.Children.Add(root);
                SplitChild(newRoot, 0);
                _root = newRoot;
            }

            InsertNonFull(_root, book);
        }

        private void SplitChild(BNodo parent, int index)
        {
            var degree = _degree;
            var fullChild = parent.Children[index];
            var newChild = new BNodo(degree);

            int mid = (degree - 1) / 2;
            parent.Books.Insert(index, fullChild.Books[mid]);
            parent.Children.Insert(index + 1, newChild);

            newChild.Books.AddRange(fullChild.Books.GetRange(mid + 1, fullChild.Books.Count - mid - 1));
            fullChild.Books.RemoveRange(mid, fullChild.Books.Count - mid);

            if (!fullChild.IsLeaf)
            {
                newChild.Children.AddRange(fullChild.Children.GetRange(mid + 1, fullChild.Children.Count - mid - 1));
                fullChild.Children.RemoveRange(mid + 1, fullChild.Children.Count - mid - 1);
            }
        }


        private void InsertNonFull(BNodo node, Book book)
        {
            if (node.IsLeaf)
            {
                int i = node.Books.Count - 1;
                while (i >= 0 && string.Compare(book.Isbn, node.Books[i].Isbn) < 0)
                {
                    i--;
                }

                node.Books.Insert(i + 1, book);

                // Verificar el número máximo de claves
                if (node.Books.Count > _maxKeys)
                {
                    // Si el nodo está lleno, se debe dividir
                    SplitNode(node);
                }
            }
            else
            {
                int i = node.Books.Count - 1;
                while (i >= 0 && string.Compare(book.Isbn, node.Books[i].Isbn) < 0)
                {
                    i--;
                }

                i++;
                if (node.Children[i].Books.Count == _maxKeys + 1)
                {
                    SplitChild(node, i);
                    if (string.Compare(book.Isbn, node.Books[i].Isbn) > 0)
                    {
                        i++;
                    }
                }
                InsertNonFull(node.Children[i], book);
            }
        }
        
        private void SplitNode(BNodo node)
        {
            if (node == _root)
            {
                // Crear una nueva raíz
                var newRoot = new BNodo(_degree);
                newRoot.Children.Add(node);
                _root = newRoot;
                SplitChild(newRoot, 0);
            }
            else
            {
                // Si el nodo no es la raíz, dividir el nodo y redistribuir
                var parent = FindParent(_root, node);
                SplitChild(parent, parent.Children.IndexOf(node));
            }
        }
        
        private BNodo FindParent(BNodo node, BNodo child)
        {
            if (node.Children.Contains(child))
            {
                return node;
            }
            foreach (var n in node.Children)
            {
                var found = FindParent(n, child);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        

        public Book SearchByIsbn(string isbn)
        {
            return Search(_root, isbn);
        }

        private Book Search(BNodo node, string isbn)
        {
            int i = 0;
            while (i < node.Books.Count && string.Compare(isbn, node.Books[i].Isbn) > 0)
            {
                i++;
            }

            if (i < node.Books.Count && node.Books[i].Isbn == isbn)
            {
                return node.Books[i];
            }

            if (node.IsLeaf)
            {
                return null;
            }

            return Search(node.Children[i], isbn);
        }

        public List<Book> SearchByName(string name)
        {
            var results = new List<Book>();
            SearchByName(_root, name, results);
            return results;
        }

        private void SearchByName(BNodo node, string name, List<Book> results)
        {
            if (node == null) return;

            // Buscar en el nodo actual
            foreach (var book in node.Books)
            {
                if (book.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(book);
                }
            }

            // Continuar búsqueda en hijos si no es una hoja
            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    SearchByName(child, name, results);
                }
            }
        }
        
        public void PrintSearchResults(List<Book> results)
        {
            
            if (results.Count == 0)
            {
                Console.WriteLine("No se encontraron libros con ese nombre.");
                return;
            }
            
            foreach (var book in results)
            {
                Console.WriteLine($"ISBN: {book.Isbn}, Nombre: {book.Name}, Autor: {book.Author}, Categoría: {book.Category}, Precio: {book.Price}, Cantidad: {book.Quantity}");
            }
        }


        //Antes de editar
        public void Delete(string isbn)
        {
            Delete(_root, isbn);

            // Si la raíz queda vacía, debe ser reemplazada por su primer hijo si no es una hoja
            if (_root.Books.Count == 0 && !_root.IsLeaf)
            {
                _root = _root.Children[0];
            }
        }

        private void Delete(BNodo node, string isbn)
        {
            int idx = FindIndex(node, isbn);
            
            if (idx >= node.Books.Count || node.Books[idx].Isbn != isbn)
            {
                // El libro no existe en el nodo actual, retornar
                Console.WriteLine("El libro no se encontró en el nodo.");
                return;
            }

            if (node.IsLeaf)
            {
                if (idx < node.Books.Count && node.Books[idx].Isbn == isbn)
                {
                    node.Books.RemoveAt(idx);
                }
                else
                {
                    Console.WriteLine("El libro no se encontró en el nodo hoja.");
                }
            }
            else
            {
                if (idx < node.Books.Count && node.Books[idx].Isbn == isbn)
                {
                    if (node.Children[idx].Books.Count >= _degree)
                    {
                        var pred = GetPredecessor(node, idx);
                        node.Books[idx] = pred;
                        Delete(node.Children[idx], pred.Isbn);
                    }
                    else if (node.Children[idx + 1].Books.Count >= _degree)
                    {
                        var succ = GetSuccessor(node, idx);
                        node.Books[idx] = succ;
                        Delete(node.Children[idx + 1], succ.Isbn);
                    }
                    else
                    {
                        MergeChildren(node, idx);
                        Delete(node.Children[idx], isbn);
                    }
                }
                else
                {
                    var childIndex = idx < node.Books.Count ? idx : idx + 1;
                    if (node.Children[childIndex].Books.Count == _degree - 1)
                    {
                        if (childIndex > 0 && node.Children[childIndex - 1].Books.Count >= _degree)
                        {
                            BorrowFromPrev(node, childIndex);
                        }
                        else if (childIndex < node.Children.Count - 1 && node.Children[childIndex + 1].Books.Count >= _degree)
                        {
                            BorrowFromNext(node, childIndex);
                        }
                        else
                        {
                            if (childIndex == node.Children.Count - 1)
                            {
                                childIndex--;
                            }
                            MergeChildren(node, childIndex);
                        }
                    }
                    Delete(node.Children[childIndex], isbn);
                }
            }
        }
        
        private int FindIndex(BNodo node, string isbn)
        {
            int idx = 0;
            while (idx < node.Books.Count && string.Compare(isbn, node.Books[idx].Isbn) > 0)
            {
                idx++;
            }
            return idx;
        }
        
        private Book GetPredecessor(BNodo node, int idx)
        {
            var current = node.Children[idx];
            while (!current.IsLeaf)
            {
                current = current.Children[current.Children.Count - 1];
            }
            return current.Books[current.Books.Count - 1];
        }
        
        private Book GetSuccessor(BNodo node, int idx)
        {
            var current = node.Children[idx + 1];
            while (!current.IsLeaf)
            {
                current = current.Children[0];
            }
            return current.Books[0];
        }
        
        private void MergeChildren(BNodo node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx + 1];

            child.Books.Add(node.Books[idx]);
            child.Books.AddRange(sibling.Books);
            if (!child.IsLeaf)
            {
                child.Children.AddRange(sibling.Children);
            }

            node.Books.RemoveAt(idx);
            node.Children.RemoveAt(idx + 1);
        }
        
        private void BorrowFromPrev(BNodo node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx - 1];

            child.Books.Insert(0, node.Books[idx - 1]);
            node.Books[idx - 1] = sibling.Books[sibling.Books.Count - 1];

            if (!child.IsLeaf)
            {
                child.Children.Insert(0, sibling.Children[sibling.Children.Count - 1]);
                sibling.Children.RemoveAt(sibling.Children.Count - 1);
            }
            sibling.Books.RemoveAt(sibling.Books.Count - 1);
        }
        
        private void BorrowFromNext(BNodo node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx + 1];

            child.Books.Add(node.Books[idx]);
            node.Books[idx] = sibling.Books[0];

            if (!child.IsLeaf)
            {
                child.Children.Add(sibling.Children[0]);
                sibling.Children.RemoveAt(0);
            }
            sibling.Books.RemoveAt(0);
        }

        /////////////////////
        public void UpdateBookFromPatchData(Dictionary<string, object> patchData)
        {
            if (patchData.ContainsKey("isbn"))
            {
                var isbn = patchData["isbn"].ToString();
                patchData.Remove("isbn");
        
                // Obtener el libro actual
                var book = SearchByIsbn(isbn);
                if (book != null)
                {
                    // Actualizar las propiedades del libro basado en patchData usando switch
                    foreach (var kvp in patchData)
                    {
                        switch (kvp.Key.ToLower())
                        {
                            case "name":
                                book.Name = kvp.Value.ToString();
                                break;
                            case "author":
                                book.Author = kvp.Value.ToString();
                                break;
                            case "category":
                                book.Category = kvp.Value.ToString();
                                break;
                            case "price":
                                book.Price = kvp.Value.ToString();
                                break;
                            case "quantity":
                                book.Quantity = kvp.Value.ToString();
                                break;
                            default:
                                Console.WriteLine($"Campo desconocido: {kvp.Key}");
                                break;
                        }
                    }
            
                    // Imprimir el libro editado
                    ImprimirLibroEditado(book);
                }
                else
                {
                    Console.WriteLine("Libro no encontrado.");
                }
            }
            else
            {
                Console.WriteLine("ISBN no proporcionado.");
            }
        }

        
        private void ImprimirLibroEditado(Book book)
        {
            Console.WriteLine($"ISBN: {book.Isbn}, Nombre: {book.Name}, Autor: {book.Author}, Categoría: {book.Category}, Precio: {book.Price}, Cantidad: {book.Quantity}");
        }
        
        
        /////////////////
        
        
        public void PrintTree()
        {
            if (_root == null)
            {
                Console.WriteLine("El árbol está vacío.");
                return;
            }

            var queue = new Queue<(BNodo Node, int Level)>();
            queue.Enqueue((_root, 0));

            var currentLevel = 0;
            while (queue.Count > 0)
            {
                var (node, level) = queue.Dequeue();

                if (level != currentLevel)
                {
                    Console.WriteLine(); // Nueva línea para cada nivel
                    currentLevel = level;
                }

                Console.Write($"Nivel {level}: [");
                foreach (var book in node.Books)
                {
                    Console.Write($" {book.Isbn} ({book.Name}) ");
                }
                Console.Write("] ");

                foreach (var child in node.Children)
                {
                    queue.Enqueue((child, level + 1));
                }
            }

            Console.WriteLine(); // Nueva línea al final
        }
    
    }