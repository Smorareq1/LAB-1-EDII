
namespace LAB_1_EDII;

using System.Text;
using Newtonsoft.Json;

public class ArbolB
    {
        private readonly int _degree;
        private readonly int _minKeys;
        private readonly int _maxKeys;
        private BNodo _root;
        
        Dictionary<string, string > NombreIsbn = new Dictionary<string, string>();

        public ArbolB(int degree)
        {
            _degree = degree;
            _minKeys = (degree + 1) / 2 - 1;
            _maxKeys = degree - 1;
            _root = new BNodo(degree);
        }

        public void Insert(Book book)
        {
            //Inserte al diccionario
            NombreIsbn[book.Name] = book.Isbn;
            
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
        
        public void Delete(string isbn)
        {
            // Eliminar del diccionario
            if (NombreIsbn.ContainsKey(isbn))
            {
                NombreIsbn.Remove(isbn);
            }
            
            
            Delete(_root, isbn);

            // If the root node is empty, replace it with its first child if not a leaf
            if (_root.Books.Count == 0 && !_root.IsLeaf)
            {
                _root = _root.Children[0];
            }
        }

        private void Delete(BNodo node, string isbn)
        {
            int idx = FindIndex(node, isbn);

            // Check if the book is in the current node
            if (idx < node.Books.Count && node.Books[idx].Isbn == isbn)
            {
                if (node.IsLeaf)
                {
                    // Case 1: Node is a leaf, simply remove the book
                    node.Books.RemoveAt(idx);
                }
                else
                {
                    // Case 2: Node is an internal node
                    if (node.Children[idx].Books.Count >= _degree)
                    {
                        // Case 2a: Predecessor in the left child has at least degree books
                        var pred = GetPredecessor(node, idx);
                        node.Books[idx] = pred;
                        Delete(node.Children[idx], pred.Isbn);
                    }
                    else if (node.Children[idx + 1].Books.Count >= _degree)
                    {
                        // Case 2b: Successor in the right child has at least degree books
                        var succ = GetSuccessor(node, idx);
                        node.Books[idx] = succ;
                        Delete(node.Children[idx + 1], succ.Isbn);
                    }
                    else
                    {
                        // Case 2c: Both children have degree-1 books, merge them
                        MergeChildren(node, idx);
                        Delete(node.Children[idx], isbn);
                    }
                }
            }
            else
            {
                // Book is not in this node
                if (node.IsLeaf)
                {
                    //Console.WriteLine("El libro no se encontró en el nodo hoja.");
                    return;
                }

                // Determine the index of the child that holds the key
                bool flag = (idx == node.Books.Count);
                if (node.Children[idx].Books.Count == _degree - 1)
                {
                    // If the child is full, fix the node
                    if (idx != 0 && node.Children[idx - 1].Books.Count >= _degree)
                    {
                        BorrowFromPrev(node, idx);
                    }
                    else if (idx != node.Books.Count && node.Children[idx + 1].Books.Count >= _degree)
                    {
                        BorrowFromNext(node, idx);
                    }
                    else
                    {
                        if (idx != node.Books.Count)
                        {
                            MergeChildren(node, idx);
                        }
                        else
                        {
                            MergeChildren(node, idx - 1);
                        }
                    }
                }

                // Move to the next level
                if (flag && idx > node.Books.Count)
                {
                    Delete(node.Children[idx - 1], isbn);
                }
                else
                {
                    Delete(node.Children[idx], isbn);
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

            // Merge the book from the node into the left child
            child.Books.Add(node.Books[idx]);

            // Append sibling's books and children to the child
            child.Books.AddRange(sibling.Books);
            if (!child.IsLeaf)
            {
                child.Children.AddRange(sibling.Children);
            }

            // Remove the book and the right child from the node
            node.Books.RemoveAt(idx);
            node.Children.RemoveAt(idx + 1);
        }

        private void BorrowFromPrev(BNodo node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx - 1];

            // Move separator book from node to the front of child
            child.Books.Insert(0, node.Books[idx - 1]);

            // Move sibling's last book to the node
            node.Books[idx - 1] = sibling.Books[sibling.Books.Count - 1];

            // Move sibling's last child to the front of child's children
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

            // Move separator book from node to the end of child
            child.Books.Add(node.Books[idx]);

            // Move sibling's first book to the node
            node.Books[idx] = sibling.Books[0];

            // Move sibling's first child to the end of child's children
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
                    string oldName = book.Name; // Guarda el nombre anterior del libro

                    // Actualizar las propiedades del libro basado en patchData usando switch
                    foreach (var kvp in patchData)
                    {
                        switch (kvp.Key.ToLower())
                        {
                            case "name":
                                book.Name = kvp.Value.ToString();
                                
                                // Verificar si el nombre cambió, para actualizar el diccionario NombreIsbn
                                if (NombreIsbn.ContainsKey(oldName))
                                {
                                    // Actualiza el diccionario con el nuevo nombre
                                    NombreIsbn.Remove(oldName);  // Remover la entrada con el nombre antiguo
                                    NombreIsbn[book.Name] = isbn; // Agregar la nueva entrada
                                }
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
            }
            else
            {
                Console.WriteLine("ISBN no proporcionado.");
            }
        }


        
        private void ImprimirLibroEditado(Book book)
        {
            Console.WriteLine( "Libro editado correctamente: " + $"ISBN: {book.Isbn}, Nombre: {book.Name}, Autor: {book.Author}, Categoría: {book.Category}, Precio: {book.Price}, Cantidad: {book.Quantity}");
        }
        
        
        /////////////////
        public void BuscarPorNombre(string name, StreamWriter writer)
        {
            
        
            if (NombreIsbn.TryGetValue(name, out var isbn))
            {
                Book book = SearchByIsbn(isbn);
                if (book != null)
                {
                    var bookJson = new
                    {
                        isbn = book.Isbn,
                        name = book.Name,
                        author = book.Author,
                        category = book.Category,
                        price = book.Price,
                        quantity = book.Quantity,
                    };
            
                    var json = JsonConvert.SerializeObject(bookJson);

                    writer.WriteLine(json); // Write the JSON string to the output file
                }
            }
            else
            {
                //Console.WriteLine($"Libro con nombre {name} no encontrado.");
            }
        }
        
        
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